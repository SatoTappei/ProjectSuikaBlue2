using System.Collections;
using System.Collections.Generic;
using System;
using System.Buffers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFCアルゴリズム周りは専用の名前空間にまとめてある。
using PSB.Game.SAW; // 自己回避ランダムウォークアルゴリズムは専用の名前空間にまとめてある。
using VContainer;

namespace PSB.Game
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] TileBuilder _tileBuilder;
        [SerializeField] EntityCreator _entityCreator;
        [Header("デバッグ用:ダンジョンのみで動作")]
        [SerializeField] bool _debug = true;

        GameState _gameState;
        Dungeon _dungeon;
        SelfAvoidingWalk _walk;
        DungeonParameterSettings _settings;
        
        [Inject]
        void Construct(GameState gameState, DungeonParameterSettings settings)
        {
            _gameState = gameState;
            _settings = settings;
        }

        void Awake()
        {
            Init();      
        }

        void Start()
        {
            if (_debug)
            {
                BuildAsync(this.GetCancellationTokenOnDestroy()).Forget();
                Debug.LogWarning("ダンジョンのみで実行中");
            }
        }

        void Init()
        {
            // タイルを生成してそれにダンジョンのグリッドを合わせる。
            // 2つのタイルの間に1つのセルが存在するので、サイズそのままだと1つ足りなくなってしまう。
            int h = _settings.Size - 1;
            int w = _settings.Size - 1;

            _dungeon = new(h, w, _settings.CellSize);

            uint seed = _settings.WalkRandomSeed ? Utility.RandomSeed() : _settings.WalkSeed;
            _walk = new(h, w, seed);
        }

        /// <summary>
        /// ダンジョンの生成
        /// </summary>
        public async UniTask BuildAsync(CancellationToken token)
        {
            await BuildDungeonBaseAsync(token);
            await CreatePathAsync(token);
            RemoveWallOnPath();
            CheckNeighbourCell();
            CreateLocation();
            // ダンジョン生成に使うクラスは完成したらメモリを解放しても良いかもしれない
        }

        // ベースとなるダンジョンを作る
        async UniTask BuildDungeonBaseAsync(CancellationToken token)
        {
            // シード値を決めて生成
            uint seed = _settings.WfcRandomSeed ? Utility.RandomSeed() : _settings.WfcSeed;
            int size = _settings.Size;
            Logic logic = new(size, size, seed);

            for (int i = 0; i < size * size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_settings.StepSpeed, cancellationToken: token);
            }
        }

        // スタートからゴールへ一本道を作る
        async UniTask CreatePathAsync(CancellationToken token)
        {
            // 任意の位置からスタート
            _walk.SetStartPoint(_settings.Entry.y, _settings.Entry.x);

            for (int i = 0; i < _walk.GridLength; i++)
            {
                // これ以上進めなくなった場合は打ち切る
                if (!_walk.Step()) break;
                await UniTask.WaitForSeconds(_settings.StepSpeed, cancellationToken: token);
            }
        }

        // スタートからゴールまでの経路にある壁を削除する
        void RemoveWallOnPath()
        {
            RaycastHit[] hit = ArrayPool<RaycastHit>.Shared.Rent(_settings.CellRayHitMax);
            Vector3 prev = default;
            bool first = true;
            // 経路の各セル同士をレイキャスト
            foreach(SelfAvoidingWalk.Cell c in _walk.Path)
            {
                // アルゴリズムのクラスは実際の座標を保持していないので座標に変換
                Vector3 current = Dungeon.IndexToPosition(c.Index, _settings.CellSize);

                if (!first)
                {
                    Vector3 origin = prev + _settings.CellRayOffset;
                    Vector3 dir = (current - prev).normalized;
                    float dist = (current - prev).magnitude;
                    Physics.RaycastNonAlloc(origin, dir, hit, dist);
                    foreach (RaycastHit h in hit)
                    {
                        if (h.collider == null) break;
                        if (!h.collider.TryGetComponent(out IDungeonParts parts)) continue;

                        // セルの中心を基準とした位置同士をレイキャストしているので
                        // 2つ以上の子オブジェクトを消したい場合は重なり合う用にコライダーを設置する必要がある。
                        parts.Remove();
                    }
                }

                prev = current;
                first = false;
            }

            Array.Clear(hit, 0, hit.Length);
            ArrayPool<RaycastHit>.Shared.Return(hit);
        }

        // 全てのセルを上下左右の隣接するセルに進めるかを判定
        void CheckNeighbourCell()
        {
            foreach (IReadOnlyCell cell in _dungeon.Grid)
            {
                foreach(Vector2Int neighbour in Direction())
                {
                    int nx = cell.Index.x + neighbour.x;
                    int ny = cell.Index.y + neighbour.y;              
                    if (!Utility.CheckInLength(_dungeon.Grid, ny, nx)) continue;

                    // 1つのセルが4つのタイルそれぞれの一部から構成されている性質上
                    // タイルの種類からそのセルが上下左右に進めるかの判定が厳しいため、レイキャストで判定する。
                    Vector3 origin = cell.Position + _settings.CellRayOffset;
                    Vector3 dir = new Vector3(neighbour.x, 0, neighbour.y);
                    float dist = (cell.Position - _dungeon.Grid[ny, nx].Position).magnitude;
                    if (Physics.Raycast(origin, dir, dist)) continue;

                    // 隣のセルにレイキャストがヒットしなかった場合は接続されている。
                    _dungeon.Connect(cell.Index.x, cell.Index.y, nx, ny);
                }
            }

            IEnumerable<Vector2Int> Direction()
            {
                yield return Vector2Int.up;
                yield return Vector2Int.down;
                yield return Vector2Int.left;
                yield return Vector2Int.right;
            }
        }

        // 場所の生成
        void CreateLocation()
        {
            // 経路の端にスタートとゴール
            //Vector3 s = new Vector3(_entry)
            //_entityCreator.Location(LocationKey.Start,)
        }

        /// <summary>
        /// ダンジョンのスタート位置を返す。
        /// この位置から必ずゴールにたどり着く事が出来る。
        /// </summary>
        public Vector3 StartPosition() => Dungeon.IndexToPosition(_settings.Entry, _settings.CellSize);

        /// <summary>
        /// ダンジョンのスタート位置のインデックスを返す。
        /// この位置から必ずゴールにたどり着く事が出来る。
        /// </summary>
        public Vector2Int StartIndex() => _settings.Entry;

        /// <summary>
        /// 移動可能かをチェックし、可能な場合は移動先を返す
        /// </summary>
        public bool TryGetMovablePosition(Vector2Int origin, Vector2Int direction, out Vector3 position)
        {
            // 上下左右の単位ベクトルに制限していないので2セル以上の移動も可能
            Vector2Int n = origin + direction;
            
            if (_dungeon.IsConnected(origin.x, origin.y, n.x, n.y))
            {
                position = _dungeon.Grid[n.y, n.x].Position;
                return true;
            }

            position = _dungeon.Grid[origin.y, origin.x].Position;
            return false;
        }

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
            if (_walk != null) _walk.DrawGridOnGizmos(_settings.CellSize);
        }
    }
}