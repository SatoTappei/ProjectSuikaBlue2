using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using PSB.Game.FSM; // ステートマシン周りは専用の名前空間にまとめてある
using StateKey = PSB.Game.FSM.State.StateKey;
using PSB.Game.BT;

namespace PSB.Game
{
    public class Enemy : MonoBehaviour, ICharacter
    {
        /// <summary>
        /// アニメーション再生用のキー
        /// </summary>
        public enum AnimationKey { Idle, Kick, Walk, Sprint }
        
        /// <summary>
        /// パーティクル再生用のキー
        /// </summary>
        public enum ParticleKey { Dash }

        [Header("モデルを回転させる")]
        [SerializeField] Transform _model;
        [Header("アニメーションの設定")]
        [SerializeField] Animator _animator;
        [SerializeField] string _idleAnimationName = "Idle";
        [SerializeField] string _kickAnimationName = "Kick";
        [SerializeField] string _walkAnimationName = "Walk";
        [SerializeField] string _sprintAnimationName = "Sprint";
        [Header("各ステート")]
        [SerializeField] IdleState _idleState;
        [SerializeField] ChaseState _chaseState;
        [SerializeField] AttackState _attackState;
        [SerializeField] SearchState _searchState;
        [Header("どのステートから開始するか")]
        [SerializeField] StateKey _defaultState = StateKey.Idle;
        [Header("生成時設定")]
        [SerializeField] Vector2Int _spawnIndex;
        [SerializeField] Direction _spawnDirection;
        [Header("地面からの高さ")]
        [SerializeField] float _groundOffset;
        [Header("水平にレイキャストする際に使う眼の高さ")]
        [SerializeField] float _eyeHeight = 0.5f;
        [Header("走る際のパーティクル")]
        [SerializeField] GameObject _particle;
        [SerializeField] Vector3 _particleOffset;
        [SerializeField] int _particleCapacity;

        GameState _gameState;
        DungeonManager _dungeonManager;
        BlackBoard _blackBoard;
        BlackBoard.Private _privateBoard;
        Transform _transform;
        Dictionary<StateKey, State> _states = new(4);
        ObjectPool _particlePool;
        Vector2Int _currentIndex;
        Direction _forward;
        int _idleAnimationHash;
        int _kickAnimationHash;
        int _walkAnimationHash;
        int _sprintAnimationHash;

        /// <summary>
        /// ビヘイビアツリーの各ノードが読み書きする。
        /// </summary>
        public BlackBoard.Private PrivateBoard => _privateBoard ??= _blackBoard.CreatePrivate();

        [Inject]
        void Construct(GameState gameState, DungeonManager dungeonManager, BlackBoard blackBoard)
        {
            _gameState = gameState;
            _dungeonManager = dungeonManager;
            _blackBoard = blackBoard;
        }

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            _transform  = transform;

            // アニメーションのハッシュ
            _idleAnimationHash = Animator.StringToHash(_idleAnimationName);
            _kickAnimationHash = Animator.StringToHash(_kickAnimationName);
            _walkAnimationHash = Animator.StringToHash(_walkAnimationName);
            _sprintAnimationHash = Animator.StringToHash(_sprintAnimationName);

            // キャラクター毎の黒板
            _privateBoard = _blackBoard.CreatePrivate();

            // ステートを辞書で管理
            _states.Add(StateKey.Idle, _idleState);
            _states.Add(StateKey.Chase, _chaseState);
            _states.Add(StateKey.Attack, _attackState);
            _states.Add(StateKey.Search, _searchState);

            // 各ステートの初期化処理
            foreach (KeyValuePair<StateKey, State> state in _states)
            {
                state.Value.Awake(gameObject);
            }

            // パーティクル用のオブジェクトプール
            _particlePool = new(_particle, _particleCapacity);

            // 初期位置
            Vector3 v = _dungeonManager.GetCell(_spawnIndex).Position;
            v.y += _groundOffset;
            SetPosition(v, _spawnIndex);

            // 回転
            Rotate(_spawnDirection);
            _forward = _spawnDirection;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 任意のステートから開始
            State state = _states[_defaultState];

            while (!token.IsCancellationRequested)
            {
                state = state.Update();
                Debug.Log(state.ToString());

                await UniTask.Yield(token);
            }
        }

        /// <summary>
        /// 遷移先のステートを取得
        /// </summary>
        public bool TryGetState(StateKey key, out State state)
        {
            return _states.TryGetValue(key, out state);
        }

        /// <summary>
        /// セル単位の距離でプレイヤーが存在するかを判定。
        /// 必要に応じてレイキャストで見えているかを判定。
        /// </summary>
        public bool DetectPlayer(int cellDistance, bool checkWithinSight)
        {
            // チェビシェフ距離でセル単位の距離を測る。
            if (_currentIndex.ChebyshevDistance(_gameState.PlayerIndex) > cellDistance) return false;

            // 自身の位置とプレイヤーが存在するセルの間をレイキャストし、
            // 障害物が無い場合は見えていると判定する。
            if (checkWithinSight)
            {
                // 視界を遮らない障害物を考慮して、眼の高さから水平にレイキャストする。
                Vector3 p = _dungeonManager.GetCell(_gameState.PlayerIndex).Position;
                p.y = _eyeHeight;
                Vector3 q = _transform.position;
                q.y = _eyeHeight;

                bool hit = Physics.Linecast(p, q, Const.DungeonLayer);
#if UNITY_EDITOR
                Debug.DrawLine(p, q, hit ? Color.red : Color.green);
#endif
                return !hit;
            }

            return true;
        }

        /// <summary>
        /// 隣接するランダムなセルへの経路探索。
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> PathfindingToNeighbour()
        {
            IReadOnlyList<IReadOnlyCell> adjacent = _dungeonManager.GetCell(_currentIndex).Adjacent;
            Vector2Int neighbour = adjacent[Random.Range(0, adjacent.Count)].Index;

            return Pathfinding(neighbour);
        }

        /// <summary>
        /// 現在地からプレイヤーまでの経路探索。
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> PathfindingToPlayer()
        {
            return Pathfinding(_gameState.PlayerIndex);
        }

        /// <summary>
        /// 現在地からの経路探索。
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> Pathfinding(Vector2Int target)
        {
            return _dungeonManager.Pathfinding(_currentIndex, target);
        }

        /// <summary>
        /// 座標を変更することで移動する。
        /// ワールド座標とは別にグリッド上の位置を管理しているので必ずセットで扱う。
        /// </summary>
        public void SetPosition(Vector3 position, Vector2Int index)
        {
            // セルに登録しているキャラクター情報も更新
            _dungeonManager.SetCharacter(CharacterKey.Dummy, _currentIndex, null);
            
            _transform.position = position;
            _currentIndex = index;

            _dungeonManager.SetCharacter(CharacterKey.Enemy, _currentIndex, this);
        }

        /// <summary>
        /// ワールド座標とグリッド上の座標を返す。
        /// ワールド座標とは別にグリッド上の位置を管理しているので必ずセットで扱う。
        /// </summary>
        public (Vector3 position, Vector2Int index) GetPosition()
        {
            return (_transform.position, _currentIndex);
        }

        /// <summary>
        /// キャラクターのモデルを任意の方向に線形補完を用いて1フレーム分回転させる。
        /// 前方向を示す値もその方向になる。
        /// </summary>
        public void Rotate(Direction direction, float t = 1)
        {
            Quaternion q = direction.ToQuaternion();
            _model.rotation = Quaternion.Lerp(_model.rotation, q, t);
            _forward = direction;
        }

        /// <summary>
        /// アニメーションを再生
        /// </summary>
        public void PlayAnimation(AnimationKey key)
        {
            if (key == AnimationKey.Idle) _animator.Play(_idleAnimationHash);
            else if (key == AnimationKey.Kick) _animator.Play(_kickAnimationHash);
            else if (key == AnimationKey.Walk) _animator.Play(_walkAnimationHash);
            else if (key == AnimationKey.Sprint) _animator.Play(_sprintAnimationHash);
        }

        /// <summary>
        /// プレイヤーに対して攻撃を行う
        /// </summary>
        public void Attack()
        {
            // 前向きを基準に方向を決める
            Vector2Int target = _currentIndex + _forward.ToIndex();
            IReadOnlyCell cell = _dungeonManager.GetCell(target);

            if (cell.Character != null) cell.Character.Damage();
        }

        /// <summary>
        /// セルに自分以外のキャラクターが存在するかを調べる。
        /// </summary>
        public bool IsExistOtherCharacter(Vector2Int index)
        {
            IReadOnlyCell cell = _dungeonManager.GetCell(index);

            // キャラクターがいない場合
            if (cell.Character == null) return false;
            // 自分以外のキャラクターかどうか
            return cell.Character != (ICharacter)this;
        }

        /// <summary>
        /// パーティクルを再生
        /// </summary>
        public void PlayParticle(ParticleKey _) 
        {
            // 現状パーティクルが1つなので引数の必要ないが一応。

            // パーティクルは再生終了後に非アクティブになるので、戻す処理が必要ない。
            GameObject p = _particlePool.Rent();
            if (p != null)
            {
                p.GetComponent<ParticleSystem>().Play();
                p.transform.position = _transform.position + _particleOffset;
            }
        }

        // ダメージ
        void IDamageReceiver.Damage()
        {
            // ここで処理するのではなく、ダメージの内容をキューイングして任意のタイミングで処理した方が良い
            Debug.Log(name + "がダメージを受けた");
        }
    }
}
