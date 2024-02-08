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
    public class DungeonManager
    {
        GameState _gameState;
        Dungeon _dungeon;
        SelfAvoidingWalk _walk;
        DungeonParameterSettings _settings;
        TileBuilder _tileBuilder;
        EntityCreator _entityCreator;

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
                    Vector3 dir = (current - prev).normalized;
                    float dist = (current - prev).magnitude;
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
                    float dist = (cell.Position - _dungeon.Position(neighbour)).magnitude;
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
            _entityCreator.Location(LocationKey.Start, _dungeon.Position(_settings.Entry));
            // 経路の現在位置が経路の末端つまりゴールになっている
            _entityCreator.Location(LocationKey.Chest, _dungeon.Position(_walk.Current));

            _dungeon.SetLocation(_settings.Entry, LocationKey.Start);
            _dungeon.SetLocation(_walk.Current, LocationKey.Chest);
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
        /// セルがどのような場所なのかを決めるキーをセット
        /// </summary>
        public void SetLocation(Vector2Int index, LocationKey key) => _dungeon.SetLocation(index, key);

        /// <summary>
        /// セルにあるアイテムを判定するためのキーをセット
        /// </summary>
        public void SetItem(Vector2Int index, ItemKey key) => _dungeon.SetItem(index, key);

        /// <summary>
        /// セルにいるキャラクターを判定するためのキーを返す
        /// </summary>
        public void SetCharacter(Vector2Int index, CharacterKey key) => _dungeon.SetCharacter(index, key);

        /// <summary>
        /// セルがどのような場所なのかを決めるキーを返す
        /// </summary>
        public LocationKey GetLocation(Vector2Int index) => _dungeon.GetLocation(index);

        /// <summary>
        /// セルにあるアイテムを判定するためのキーを返す
        /// </summary>
        public ItemKey GetItem(Vector2Int index) => _dungeon.GetItem(index);

        /// <summary>
        /// セルにいるキャラクターを判定するためのキーを返す
        /// </summary>
        public CharacterKey GetCharacter(Vector2Int index) => _dungeon.GetCharacter(index);

        /// <summary>
        /// 移動可能かをチェックし、可能な場合は移動先を返す
        /// </summary>
        public bool TryGetMovablePosition(Vector2Int origin, Vector2Int direction, out Vector3 position)
        {
            // 上下左右の単位ベクトルに制限していないので2セル以上の移動も可能
            Vector2Int n = origin + direction;
            
            if (_dungeon.IsConnected(origin, n))
            {
                position = _dungeon.Position(n);
                return true;
            }

            position = _dungeon.Position(origin);
            return false;
        }

        /// <summary>
        /// ダンジョンの情報をギズモに描画
        /// </summary>
        public void DrawOnGizmos()
        {
            _dungeon.DrawGridOnGizmos();
            _walk.DrawGridOnGizmos(_settings.CellSize);
        }
    }
}