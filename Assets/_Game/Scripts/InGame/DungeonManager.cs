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
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] TileBuilder _tileBuilder;
        [SerializeField] EntityCreator _entityCreator;
        [Header("ダンジョン生成設定")]
        [SerializeField] int _size = 20;
        [SerializeField] float _cellSize = 1;
        [SerializeField] Vector2Int _entry;
        [Header("WFC設定")]
        [Min(1)]
        [SerializeField] uint _wfcSeed = 1;
        [SerializeField] bool _wfcRandomSeed;
        [Header("自己回避ウォーク設定")]
        [Min(1)]
        [SerializeField] uint _walkSeed = 1;
        [SerializeField] bool _walkRandomSeed;
        [Header("セル間のレイキャストの設定")]
        [SerializeField] Vector3 _cellRayOffset = Vector3.up;
        [SerializeField] int _cellRayHitMax = 9;
        [Header("更新速度")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Dungeon _dungeon;
        SelfAvoidingWalk _walk;

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // タイルを生成してそれにダンジョンのグリッドを合わせる。
            // 2つのタイルの間に1つのセルが存在するので、サイズそのままだと1つ足りなくなってしまう。
            int h = _size - 1;
            int w = _size - 1;

            _dungeon = new(h, w, _cellSize);

            uint seed = _walkRandomSeed ? Utility.RandomSeed() : _walkSeed;
            _walk = new(h, w, seed);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
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
            uint seed = _wfcRandomSeed ? Utility.RandomSeed() : _wfcSeed;
            Logic logic = new(_size, _size, seed);

            for (int i = 0; i < _size * _size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        // スタートからゴールへ一本道を作る
        async UniTask CreatePathAsync(CancellationToken token)
        {
            // 任意の位置からスタート
            _walk.SetStartPoint(_entry.y, _entry.x);

            for (int i = 0; i < _walk.GridLength; i++)
            {
                // これ以上進めなくなった場合は打ち切る
                if (!_walk.Step()) break;
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        // 作った経路をダンジョンに反映させる
        void CopyPathToDungeon()
        {
            foreach(SelfAvoidingWalk.Cell c in _walk.Path)
            {

            }
        }

        // スタートからゴールまでの経路にある壁を削除する
        void RemoveWallOnPath()
        {
            RaycastHit[] hit = ArrayPool<RaycastHit>.Shared.Rent(_cellRayHitMax);
            Vector3 prev = default;
            bool first = true;
            // 経路の各セル同士をレイキャスト
            foreach(SelfAvoidingWalk.Cell c in _walk.Path)
            {
                // アルゴリズムのクラスは実際の座標を保持していないので座標に変換
                Vector3 current = Dungeon.IndexToPosition(c.Index, _cellSize);

                if (!first)
                {
                    Vector3 dir = (current - prev).normalized;
                    float dist = (current - prev).magnitude;
                    Physics.RaycastNonAlloc(prev + _cellRayOffset, dir, hit, dist);
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
                    Vector3 dir = new Vector3(neighbour.x, 0, neighbour.y);
                    float dist = (cell.Position - _dungeon.Grid[ny, nx].Position).magnitude;
                    if (Physics.Raycast(cell.Position + _cellRayOffset, dir, dist)) continue;

                    // 隣のセルにレイキャストがヒットしなかった場合は接続されている。
                    _dungeon.ConnectCell(cell.Index.x, cell.Index.y, nx, ny);
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

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
            if (_walk != null) _walk.DrawGridOnGizmos(_cellSize);
        }
    }
}