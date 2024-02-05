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
                Adjacent = new(4); // �㉺���E
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
        /// �ǂݎ���p�̑S�ẴZ��
        /// </summary>
        public IReadOnlyCell[,] Grid => _grid;

        public Dungeon(int width, int height, float cellSize)
        {
            _cellSize = cellSize;
            Create(height, width, cellSize);
        }

        // �O���b�h�𐶐�
        void Create(int height, int width, float cellSize)
        {
            _grid = new Cell[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // ������(0, 0)�ɂȂ��Ă���Axz���ʏ��z��������y�Ax��������x�ɂ��Ă���B
                    // Unity��xz���ʂ̍��W�n�Ɠ����B
                    _grid[y, x] = new Cell(x, y, IndexToPosition(x, y, cellSize));
                }
            }
        }

        /// <summary>
        /// �O���b�h���M�Y���ɕ`��
        /// </summary>
        public void DrawGridOnGizmos()
        {
            if (_grid == null) return;

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    // �T�C�Y��y�͓K��
                    Gizmos.DrawWireCube(_grid[i, k].Position, new Vector3(_cellSize, 0.05f, _cellSize));
                }
            }

            DrawAdjacentOnGizmos();
        }

        /// <summary>
        /// 2�̃Z�����m���q����
        /// </summary>
        public void ConnectCell(int x1, int y1, int x2, int y2)
        {
            _grid[y1, x1].Adjacent.Add(_grid[y2, x2]);
            _grid[y2, x2].Adjacent.Add(_grid[y1, x1]);
        }

        // �אڃ��X�g���M�Y���ɕ`��
        void DrawAdjacentOnGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Cell c in _grid)
            {
                foreach(IReadOnlyCell d in c.Adjacent)
                {
                    Gizmos.DrawLine(c.Position, d.Position);
                }
            }
        }

        /// <summary>
        /// �Y���������W�ɕϊ�
        /// </summary>
        public static Vector3 IndexToPosition(int x, int y, float cellSize)
        {
            float px = x * cellSize + cellSize / 2;
            float py = y * cellSize + cellSize / 2;
            return new Vector3(px, 0, py);
        }

        /// <summary>
        /// �Y���������W�ɕϊ�
        /// </summary>
        public static Vector3 IndexToPosition(Vector2Int index, float cellSize)
        {
            return IndexToPosition(index.x, index.y, cellSize);
        }
    }
}
