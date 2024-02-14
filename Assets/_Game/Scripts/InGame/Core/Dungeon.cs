using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using YamlDotNet.Core.Tokens;
using System;
using UnityEngine.UIElements;

namespace PSB.Game
{
    public partial class Dungeon
    {
        #region ダンジョンを構成するセルのクラス
        class Cell : IReadOnlyCell
        {
            public Cell(int x, int y, Vector3 position)
            {
                Index = new Vector2Int(x, y);
                Position = position;
            }

            public Vector2Int Index { get; private set; }
            public Vector3 Position { get; private set; }
            public LocationKey LocationKey { get; set; }
            public ItemKey ItemKey { get; set; }
            public CharacterKey CharacterKey { get; set; }
            public ILocation Location { get; set; }
            public IItem Item { get; set; }
            public ICharacter Character { get; set; }
            public List<IReadOnlyCell> Adjacent { get; set; } = new(4); // 上下左右

            IReadOnlyList<IReadOnlyCell> IReadOnlyCell.Adjacent => Adjacent;
        }
        #endregion

        float _cellSize;
        Cell[,] _grid;

        public IReadOnlyCell this[Vector2Int index] => _grid[index.y, index.x];

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
        public void Connect(Vector2Int a, Vector2Int b)
        {
            if (!Utility.CheckInLength(_grid, a)) return;
            if (!Utility.CheckInLength(_grid, b)) return;

            _grid[a.y, a.x].Adjacent.Add(_grid[b.y, b.x]);
            _grid[b.y, b.x].Adjacent.Add(_grid[a.y, a.x]);
        }

        /// <summary>
        /// 2つのセル同士が接続されているか
        /// </summary>
        public bool IsConnected(Vector2Int a, Vector2Int b)
        {
            if (!Utility.CheckInLength(_grid, a)) return false;
            if (!Utility.CheckInLength(_grid, b)) return false;

            // 無効グラフなので片方調べればよい
            return _grid[a.y, a.x].Adjacent.Contains(_grid[b.y, b.x]);
        }

        /// <summary>
        /// セルのロケーションをセット
        /// </summary>
        public void SetLocation(LocationKey key, Vector2Int index, ILocation location)
        {
            if (!Utility.CheckInLength(_grid, index)) return;

            _grid[index.y, index.x].LocationKey = key;
            _grid[index.y, index.x].Location = location;
        }

        /// <summary>
        /// セルにあるアイテムをセット
        /// </summary>
        public void SetItem(ItemKey key, Vector2Int index, IItem item)
        {
            if (!Utility.CheckInLength(_grid, index)) return;

            _grid[index.y, index.x].ItemKey = key;
            _grid[index.y, index.x].Item = item;
        }

        /// <summary>
        /// セルにいるキャラクターをセット
        /// </summary>
        public void SetCharacter(CharacterKey key, Vector2Int index, ICharacter character)
        {
            if (!Utility.CheckInLength(_grid, index)) return;

            _grid[index.y, index.x].CharacterKey = key;
            _grid[index.y, index.x].Character = character;
        }

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
