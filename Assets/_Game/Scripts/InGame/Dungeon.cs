using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public class Dungeon
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
            public LocationKey Location { get; set; }
            public ItemKey Item { get; set; }
            public CharacterKey Character { get; set; }
        }

        float _cellSize;
        Cell[,] _grid;
        
        public Dungeon(int width, int height, float cellSize)
        {
            _cellSize = cellSize;
            Create(height, width, cellSize);
        }

        // �O���b�h�𐶐�
        void Create(int height, int width, float cellSize)
        {
            _grid = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    float px = i * _cellSize + _cellSize / 2;
                    float py = k * _cellSize + _cellSize / 2;
                    // ������(0, 0)�ɂȂ��Ă���Axz���ʏ��z��������y�Ax��������x�ɂ��Ă���B
                    // Unity��xz���ʂ̍��W�n�Ɠ����B
                    _grid[i, k] = new(i, k, new Vector3(py, 0, px));
                }
            }
        }

        public void CheckCell(int y, int x)
        {
            Vector3 p = _grid[y, x].Position;
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
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
        }
    }
}
