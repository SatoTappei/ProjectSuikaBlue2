using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace PSB.Game.SAW
{
    public class SelfAvoidingWalk
    {
        class Cell
        {
            public Cell(int y, int x, Vector3 position)
            {
                Index = new Vector2Int(y, x);
                Position = position;
            }

            public Vector2Int Index { get; private set; }
            public Vector3 Position { get; private set; }
            public bool Visited { get; set; }
        }

        float _cellSize;
        Cell[,] _grid;
        Random _random;
        Vector2Int _current;

        public SelfAvoidingWalk(int width, int height, float cellSize, uint seed)
        {
            _cellSize = cellSize;
            Create(height, width, cellSize);
            _random = new Random(seed);
        }

        // グリッドを生成
        void Create(int height, int width, float cellSize)
        {
            _grid = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    float px = i * _cellSize + _cellSize / 2;
                    float py = k * _cellSize + _cellSize / 2;
                    // 左下が(0, 0)になっており、xz平面上のz軸方向をy、x軸方向をxにしてある。
                    // Unityのxz平面の座標系と同じ。
                    _grid[i, k] = new(i, k, new Vector3(py, 0, px));
                }
            }
        }

        /// <summary>
        /// 開始座標をセット
        /// </summary>
        public void SetStartPoint(int y, int x)
        {
            _current.y = y;
            _current.x = x;
            _grid[y, x].Visited = true;
        }

        /// <summary>
        /// 1回分進める
        /// </summary>
        public void Step()
        {
            _current += RandomDirection();
            _current = ClampInGrid(_current);
            _grid[_current.y, _current.x].Visited = true;
        }

        Vector2Int RandomDirection()
        {
            int r = _random.NextInt(0, 4);
            if (r == 0) return Vector2Int.up;
            else if (r == 1) return Vector2Int.down;
            else if (r == 2) return Vector2Int.left;
            else return Vector2Int.right;
        }

        Vector2Int ClampInGrid(Vector2Int position)
        {
            int y = Mathf.Clamp(position.y, 0, _grid.GetLength(0) - 1);
            int x = Mathf.Clamp(position.x, 0, _grid.GetLength(1) - 1);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// グリッドをギズモに描画
        /// </summary>
        public void DrawGridOnGizmos()
        {
            if (_grid == null) return;

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    Gizmos.color = _grid[i, k].Visited ? Color.red : Color.white;
                    // サイズのyは適当
                    Gizmos.DrawWireSphere(_grid[i, k].Position, 1.0f);
                }
            }
        }
    }
}
