using System.Collections;
using System.Collections.Generic;
using System;
using System.Buffers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFCアルゴリズム周りは専用の名前空間にまとめてある。
using PSB.Game.SAW; // 自己回避ランダムウォークアルゴリズムは専用の名前空間にまとめてある。

namespace PSB.Game
{
    /// <summary>
    /// ダンジョンとそこに設置されるオブジェクトの生成を行う。
    /// ダンジョンの各セルの値の読み書き、経路探索機能を外部に提供する。
    /// 必要なクラスにDIして使う。
    /// </summary>
    public class DungeonManager
    {
        GameState _gameState;
        Dungeon _dungeon;
        AStar _aStar;
        SelfAvoidingWalk _walk;
        DungeonParameterSettings _settings;
        TileBuilder _tileBuilder;
        EntityCreator _entityCreator;

        // A*で求めた経路をギズモに描画する用途
        List<IReadOnlyCell> _pathCopy = new();

        /// <summary>
        /// ダンジョンの1辺の直径。
        /// </summary>
        public float Diameter => _settings.CellSize * _settings.Size;
        /// <summary>
        /// ダンジョンの1辺のセルの数。
        /// </summary>
        public int Size => _settings.Size - 1;

        public DungeonManager(GameState gameState, DungeonParameterSettings settings,
            TileBuilder tileBuilder, EntityCreator entityCreator)
        {
            _gameState = gameState;
            _settings = settings;
            _tileBuilder = tileBuilder;
            _entityCreator = entityCreator;

            // タイルを生成してそれにダンジョンのグリッドを合わせる。
            // 2つのタイルの間に1つのセルが存在するので、サイズそのままだと1つ足りなくなってしまう。
            Init(_settings.Size - 1);
        }

        void Init(int size)
        {
            // ダンジョンの情報のクラス
            _dungeon = new(size, size, _settings.CellSize);
            // スタートからゴールまでの経路を作るアルゴリズム
            uint seed = _settings.WalkRandomSeed ? Utility.RandomSeed() : _settings.WalkSeed;
            _walk = new(size, size, seed);
            // 経路探索
            _aStar = new(_dungeon.Grid);
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

            // TODO:ダンジョン生成に使うクラスは完成したらメモリを解放しても良いかもしれない
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
            _walk.SetStartPoint(_settings.Entry);
            _gameState.StartIndex = _settings.Entry;

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
                    Vector3 dir = MyMath.Normalize(current - prev);
                    float dist = MyMath.Magnitude(current - prev);
                    Physics.RaycastNonAlloc(origin, dir, hit, dist, Const.DungeonLayer);
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
                foreach(Vector2Int d in Direction())
                {
                    Vector2Int neighbour = cell.Index + d;            
                    if (!Utility.CheckInLength(_dungeon.Grid, neighbour)) continue;

                    // 1つのセルが4つのタイルそれぞれの一部から構成されている性質上
                    // タイルの種類からそのセルが上下左右に進めるかの判定が厳しいため、レイキャストで判定する。
                    Vector3 origin = cell.Position + _settings.CellRayOffset;
                    Vector3 dir = new Vector3(d.x, 0, d.y);
                    float dist = (cell.Position - _dungeon[neighbour].Position).magnitude;
                    if (Physics.Raycast(origin, dir, dist, Const.DungeonLayer)) continue;

                    // 隣のセルにレイキャストがヒットしなかった場合は接続されている。
                    _dungeon.Connect(cell.Index, neighbour);
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
            CreateAndSet(LocationKey.Start, _settings.Entry);
            // 経路の現在位置が経路の末端つまりゴールになっている
            CreateAndSet(LocationKey.Chest, _walk.Current);

            void CreateAndSet(LocationKey key, Vector2Int index)
            {
                Vector3 p = _dungeon[index].Position;
                _dungeon.SetLocation(key, index, _entityCreator.Location(key, p));
            }
        }

        /// <summary>
        /// ダンジョンのスタート位置のセルを返す。
        /// この位置から必ずゴールにたどり着く事が出来る。
        /// </summary>
        public IReadOnlyCell GetStartCell() => _dungeon[_settings.Entry];

        /// <summary>
        /// セルを返す。
        /// </summary>e
        public IReadOnlyCell GetCell(Vector2Int index) => _dungeon[index];

        /// <summary>
        /// セルにキャラクターをセット。
        /// </summary>
        public void SetCharacter(CharacterKey key, Vector2Int index, ICharacter character)
        {
            _dungeon.SetCharacter(key, index, character);
        }

        /// <summary>
        /// 2つのセルが接続されているかを返す。
        /// </summary>
        public bool IsConnected(Vector2Int a, Vector2Int b) => _dungeon.IsConnected(a, b);

        /// <summary>
        /// 2点間の経路探索
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> Pathfinding(Vector2Int a, Vector2Int b)
        {
            List<IReadOnlyCell> path = _aStar.Pathfinding(a, b);
            // ギズモに描画する用にコピーしておく
            _pathCopy = path;

            return path;
        }

        /// <summary>
        /// ダンジョンの情報をギズモに描画
        /// </summary>
        public void DrawOnGizmos()
        {
            _dungeon.DrawGridOnGizmos();
            _walk.DrawGridOnGizmos(_settings.CellSize);
            DrawPathOnGizmos();
            DrawCharacterOnGizmos();
        }

        // 最後に求めた経路を描画
        void DrawPathOnGizmos()
        {
            if (_pathCopy == null) return;

            Gizmos.color = Color.yellow;
            foreach (IReadOnlyCell v in _pathCopy) Gizmos.DrawCube(v.Position, Vector3.one);
        }

        // キャラクターの位置を描画
        void DrawCharacterOnGizmos()
        {
            foreach (var v in _dungeon.Grid)
            {
                if (v.CharacterKey == CharacterKey.Player)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(v.Position, 1);
                }
                else if (v.CharacterKey == CharacterKey.Enemy)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(v.Position, 1);
                }
            }
        }
    }
}