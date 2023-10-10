using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = Unity.Mathematics.Random;

namespace PSB.WaveFunctionCollapse
{
    public class Logic
    {
        // 上下左右
        static readonly Vector2Int[] FourDirections =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        Rule _rule;
        Cell[,] _map;
        Random _random;

        public Logic(int height, int width, uint seed)
        {
            _rule = new();
            _random = new Random(seed);
            CreateMap(height, width);
        }

        void CreateMap(int height, int width)
        {
            _map = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    // 全てのセルはどのタイルにもなる可能性がある
                    TileType[] allTiles = { TileType.DownLeft, TileType.UpRight, TileType.DownRight, TileType.UpLeft };
                    _map[i, k] = new(new Vector2Int(k, i), allTiles);
                }
            }
        }

        public Cell[,] Step()
        {
            CollapseCell();

            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int k = 0; k < _map.GetLength(1); k++)
                {
                    if (!_map[i, k].IsCollapsed)
                    {
                        PropagateToCell(i, k);
                    }
                }
            }

            return _map;
        }

        void CollapseCell()
        {
            // 一時保存のリストに未崩壊のセルを追加する
            List<Cell> temp = new();
            foreach (Cell cell in _map)
            {
                if (!cell.IsCollapsed) temp.Add(cell);
            }
            // ランダムで崩壊するセルを決めるため、エントロピーが一番低いセル以外を省く
            temp = temp.OrderBy(c => c.SelectableTiles.Length).ToList();
            int length = temp[0].SelectableTiles.Length;
            temp = temp.Where(c => c.SelectableTiles.Length == length).ToList();
            // セルをランダムに選び、そのセルのタイル選択肢の中からランダムに1つのタイルを選んで崩壊させる
            int cellIndex = _random.NextInt(0, temp.Count);
            TileType tile = temp[cellIndex].SelectableTiles[_random.NextInt(0, temp[cellIndex].SelectableTiles.Length)];
            temp[cellIndex].Collapse(tile);
        }

        void PropagateToCell(int y, int x)
        {
            // 全てのタイルを選択肢に含める
            List<TileType> selectableTiles = new()
            {
                TileType.Floor, TileType.Cross, TileType.Vertical, TileType.Horizontal,
                TileType.DownLeft, TileType.UpRight, TileType.DownRight, TileType.UpLeft,
                TileType.UpT, TileType.DownT, TileType.LeftT, TileType.RightT,
            };
            // 対象のセルの上下左右のセルを調べる
            foreach (Vector2Int dir in FourDirections)
            {
                int dy = y + dir.y;
                int dx = x + dir.x;
                // マップの範囲内かチェック
                if (!IsWithinLength(dy, dx)) continue;
                Vector2Int reverse = ReverseDirection(dir);
                // 指定方向のセルの選択可能なタイルに基づいて、接続可能なタイルをvalidTilesに追加
                List<TileType> validTiles = new();
                foreach (TileType tile in _map[dy, dx].SelectableTiles)
                {
                    IReadOnlyList<TileType> tilesForDirection = _rule.GetConnectableTilesForDirection(tile, reverse);
                    validTiles = validTiles.Concat(tilesForDirection).ToList();
                }
                // 積集合を取り、有効なタイル以外を省く
                selectableTiles = selectableTiles.Intersect(validTiles).ToList();
            }
            // セルが選択できるタイルとして反映する
            _map[y, x].SelectableTiles = selectableTiles.ToArray();
        }

        Vector2Int ReverseDirection(Vector2Int dir)
        {
            if (dir == Vector2Int.right) return Vector2Int.down;
            if (dir == Vector2Int.left) return Vector2Int.up;
            if (dir == Vector2Int.down) return Vector2Int.right;
            if (dir == Vector2Int.up) return Vector2Int.left;

            throw new System.ArgumentException("上下左右以外のVector2Intが引数になっている: " + dir);
        }

        bool IsWithinLength(int y, int x)
        {
            return 0 <= y && y < _map.GetLength(0) && 0 <= x && x < _map.GetLength(1);
        }
    }
}
