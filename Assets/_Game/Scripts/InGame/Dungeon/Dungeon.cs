using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public class Dungeon
    {
        public class Cell : IReadOnlyCell
        {
            public Cell(int x, int y, Vector3 position)
            {
                Index = new Vector2Int(x, y);
                Position = position;
                Adjacent = new(4); // 上下左右
            }

            public Vector2Int Index { get; private set; }
            public Vector3 Position { get; private set; }
            public LocationKey Location { get; set; }
            public ItemKey Item { get; set; }
            public CharacterKey Character { get; set; }
            public List<IReadOnlyCell> Adjacent { get; set; }
        }

        float _cellSize;
        Cell[,] _grid;

        /// <summary>
        /// 読み取り専用の全てのセル
        /// </summary>
        public IReadOnlyCell[,] Grid => _grid;

        public Dungeon(int width, int height, float cellSize)
        {
            _cellSize = cellSize;
            Create(height, width, cellSize);
        }

        // グリッドを生成
        void Create(int height, int width, float cellSize)
        {
            _grid = new Cell[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 左下が(0, 0)になっており、xz平面上のz軸方向をy、x軸方向をxにしてある。
                    // Unityのxz平面の座標系と同じ。
                    _grid[y, x] = new Cell(x, y, IndexToPosition(x, y, cellSize));
                }
            }
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
                    // サイズのyは適当
                    Gizmos.DrawWireCube(_grid[i, k].Position, new Vector3(_cellSize, 0.05f, _cellSize));
                }
            }

            DrawAdjacentOnGizmos();
        }

        // 隣接リストをギズモに描画
        void DrawAdjacentOnGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Cell c in _grid)
            {
                foreach (IReadOnlyCell d in c.Adjacent)
                {
                    Gizmos.DrawLine(c.Position, d.Position);
                }
            }
        }

        /// <summary>
        /// 無向グラフとして2つのセル同士を繋げる
        /// </summary>
        public void Connect(int x1, int y1, int x2, int y2)
        {
            _grid[y1, x1].Adjacent.Add(_grid[y2, x2]);
            _grid[y2, x2].Adjacent.Add(_grid[y1, x1]);
        }

        /// <summary>
        /// 無向グラフとして2つのセル同士を繋げる
        /// </summary>
        public void Connect(Vector2Int a, Vector2Int b) => Connect(a.x, a.y, b.x, b.y);

        /// <summary>
        /// 2つのセル同士が接続されているか
        /// </summary>
        public bool IsConnected(int x1, int y1, int x2, int y2)
        {
            if (!Utility.CheckInLength(_grid, x1, y1)) return false;
            if (!Utility.CheckInLength(_grid, x2, y2)) return false;

            // 無効グラフなので片方調べればよい
            return _grid[y1, x1].Adjacent.Contains(_grid[y2, x2]);
        }

        /// <summary>
        /// 2つのセル同士が接続されているか
        /// </summary>
        public bool IsConnected(Vector2Int a, Vector2Int b) => IsConnected(a.x, a.y, b.x, b.y);

        /// <summary>
        /// セルの位置
        /// </summary>
        public Vector3 Position(Vector2Int index) => _grid[index.y, index.x].Position;

        /// <summary>
        /// セルがどのような場所なのかを決めるキーをセット
        /// </summary>
        public void SetLocation(Vector2Int index, LocationKey key) => _grid[index.y, index.x].Location = key;

        /// <summary>
        /// セルにあるアイテムを判定するためのキーをセット
        /// </summary>
        public void SetItem(Vector2Int index, ItemKey key) => _grid[index.y, index.x].Item = key;

        /// <summary>
        /// セルにいるキャラクターを判定するためのキーを返す
        /// </summary>
        public void SetCharacter(Vector2Int index, CharacterKey key) => _grid[index.y, index.x].Character = key;

        /// <summary>
        /// セルがどのような場所なのかを決めるキーを返す
        /// </summary>
        public LocationKey GetLocation(Vector2Int index) => _grid[index.y, index.x].Location;

        /// <summary>
        /// セルにあるアイテムを判定するためのキーを返す
        /// </summary>
        public ItemKey GetItem(Vector2Int index) => _grid[index.y, index.x].Item;

        /// <summary>
        /// セルにいるキャラクターを判定するためのキーを返す
        /// </summary>
        public CharacterKey GetCharacter(Vector2Int index) => _grid[index.y, index.x].Character;

        /// <summary>
        /// 添え字を座標に変換
        /// </summary>
        public static Vector3 IndexToPosition(int x, int y, float cellSize)
        {
            float px = x * cellSize + cellSize / 2;
            float py = y * cellSize + cellSize / 2;
            return new Vector3(px, 0, py);
        }

        /// <summary>
        /// 添え字を座標に変換
        /// </summary>
        public static Vector3 IndexToPosition(Vector2Int index, float cellSize)
        {
            return IndexToPosition(index.x, index.y, cellSize);
        }
    }
}
