using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using VContainer;

namespace PSB.Game
{
    public class Turret : MonoBehaviour, ICharacter
    {
        [SerializeField] MeshDrawer _meshDrawer;
        [Header("検知範囲のコライダー")]
        [SerializeField] MeshCollider _detectArea;
        [Header("弾の発射の設定")]
        [SerializeField] Transform _muzzle;
        [SerializeField] GameObject _particle;
        [SerializeField] int _bulletCapacity = 10;
        [SerializeField] float _fireRate = 1.0f;
        [Header("回転軸となるオブジェクト")]
        [SerializeField] Transform _rotateAxis;
        [Header("弾道計算の設定")]
        [Range(0, 1.0f)]
        [SerializeField] float _quadMiddleProgress = 0.5f;
        [SerializeField] float _quadMiddleHeight = 0;
        [SerializeField] float _quadRightHeight = 0;
        [SerializeField] float _tranjectoryVertexDelta = 0.2f;
        [SerializeField] float _fireRange = 5.0f;
        [Header("プレイヤーに向く速度")]
        [SerializeField] float _lookSpeed = 1.0f;
        [Header("生成時設定")]
        [SerializeField] Vector2Int _spawnIndex;
        [SerializeField] bool _randomSpawn;
        [Header("地面からの高さ")]
        [SerializeField] float _groundOffset;
        [Header("ギズモに描画")]
        [SerializeField] bool _drawOnGizmos = true;

        GameState _gameState;
        DungeonManager _dungeonManager;
        ObjectPool _particlePool;
        Vector3 _defaultLookAt;
        Vector3 _lookAt;
        Vector2Int _currentIndex;
        // (0,0)を原点にzy平面上の二次関数のグラフ
        IReadOnlyList<Vector3> _quad;
        // プレイヤーを検知中フラグ
        bool _isDetecting;

        [Inject]
        void Construct(GameState gameState, DungeonManager dungeonManager)
        {
            _gameState = gameState;
            _dungeonManager = dungeonManager;
        }

        void Awake()
        {
            // ランダム生成フラグが立っている場合は初期化前に生成位置を変更しておく。
            if (_randomSpawn && _dungeonManager != null)
            {
                _spawnIndex = Utility.RandomIndex(_dungeonManager.Size);
            }
        }

        void Start()
        {
            Init();
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            RotateAsync(token).Forget();
            FireAsync(token).Forget();
        }

        void Init()
        {
            // 二次関数から弾道のメッシュを作成
            _quad = Quad(_fireRange);
            _meshDrawer.Line(_quad, Vector3.left);

            // 検知範囲にプレイヤーが引っかかっている場合はその位置を向くように設定
            _detectArea.OnTriggerStayAsObservable()
                .Where(c => c.CompareTag(Const.PlayerTag))
                .Subscribe(c => { _lookAt = c.transform.position; _isDetecting = true; });
            // 引っかかっていない場合はデフォルトの前方向を向くように設定
            _detectArea.OnTriggerExitAsObservable()
                .Where(c => c.CompareTag(Const.PlayerTag))
                .Subscribe(c => { _lookAt = _defaultLookAt; _isDetecting = false; });

            // パーティクル用のオブジェクトプール
            _particlePool = new(_particle, _bulletCapacity);

            if (_dungeonManager == null) return;

            // 検知範囲外にプレイヤーがいる場合に向くマズルの前方向
            // プレイヤーの初期位置を向く
            _lookAt = _defaultLookAt = _dungeonManager.GetStartCell().Position;

            // 初期位置
            Vector3 v = _dungeonManager.GetCell(_spawnIndex).Position;
            v.y += _groundOffset;
            SetPosition(v, _spawnIndex);
        }

        // タレットを回転させる。
        async UniTaskVoid RotateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _rotateAxis.forward = Forward();

                await UniTask.Yield(token);
            }
        }

        // 検知中フラグが立っている場合、一定間隔で射撃。
        async UniTaskVoid FireAsync(CancellationToken token)
        {
            if (_gameState != null)
            {
                // 準備完了のフラグが立つまで射撃しない。
                await UniTask.WaitUntil(() => _gameState.IsInGameReady, cancellationToken: token);
            }

            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() => _isDetecting, cancellationToken: token);
                if (RaycastToPlayer()) Fire();
                await UniTask.WaitForSeconds(_fireRate, cancellationToken: token);
            }
        }

        // マズルの前方向にプレイヤーを検知するレイキャスト
        bool RaycastToPlayer()
        {
            return Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, _fireRange) &&
                hit.collider.CompareTag(Const.PlayerTag);
        }

        // zy平面上に二次関数のグラフを新しく配列を作って返す。
        IReadOnlyList<Vector3> Quad(float distance)
        {
            // 原点、原点から引数の距離だけx軸方向に移動した点、その間の点
            Vector2 p = Vector2.zero;
            Vector2 r = new Vector2(distance, 0);
            Vector2 q = Vector2.Lerp(p, r, _quadMiddleProgress);
            // 原点以外の2点は任意の高さに変更
            q.y = _quadMiddleHeight;
            r.y = _quadRightHeight;

            // 3頂点を通るような二次関数を計算
            Quadratic quad = new(p, q, r);

            // マズルの位置を原点としたz軸方向に伸びる曲線
            List<Vector3> list = new();
            for (float i = 0; i < distance; i += _tranjectoryVertexDelta)
            {
                i = Mathf.Min(i, distance);

                Vector3 v = Vector3.forward * i + Vector3.up * quad.GetY(i);
                list.Add(v);
            }
            
            return list;
        }

        // マズルが向く方向を計算
        Vector3 Forward()
        {
            Vector3 a = _lookAt;
            a.y = 0;
            Vector3 b = _rotateAxis.position;
            b.y = 0;
            Vector3 dir = MyMath.Normalize(a - b);

            return Vector3.Lerp(_rotateAxis.forward, dir, Time.deltaTime * _lookSpeed);
        }

        // 座標を変更することで移動する。
        // ワールド座標とは別にグリッド上の位置を管理しているので必ずセットで扱う。
        void SetPosition(Vector3 position, Vector2Int index)
        {
            if (_dungeonManager == null) return;

            // セルに登録しているキャラクター情報も更新
            _dungeonManager.SetCharacter(CharacterKey.Dummy, _currentIndex, null);

            transform.position = position;
            _currentIndex = index;

            _dungeonManager.SetCharacter(CharacterKey.Turret, _currentIndex, this);
        }

        // 射撃
        void Fire()
        {
            AudioPlayer.Play(AudioKey.TurretFireSE, AudioPlayer.PlayMode.SE);

            // パーティクルは再生終了後に非アクティブになるので、戻す処理が必要ない。
            GameObject particle = _particlePool.Rent();
            if (particle != null)
            {
                foreach (ParticleSystem p in particle.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Play();
                }
                particle.transform.position = _muzzle.transform.position;
            }

            // 複雑になるので弾道通りではなく直線のレイキャストで判定
            if(Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, _fireRange))
            {
                // プレイヤーを判定
                if(hit.collider.CompareTag(Const.PlayerTag) &&
                   hit.collider.TryGetComponent(out ICharacter player))
                {
                    player.Damage();
                }
            }
        }

        // ダメージ
        void IDamageReceiver.Damage()
        {
        }

        void OnDrawGizmos()
        {
            if(_drawOnGizmos) DrawQuadOnGizmos();
        }

        // 二次関数のグラフをギズモに描画
        void DrawQuadOnGizmos()
        {
            if (_quad == null) return;

            Gizmos.color = Color.green;
            foreach(Vector3 v in  _quad)
            {
                Gizmos.DrawSphere(v, 0.2f); // 大きさは適当
            }
        }
    }
}
