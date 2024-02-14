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
        #region �_���W�������\������Z���̃N���X
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
            public List<IReadOnlyCell> Adjacent { get; set; } = new(4); // �㉺���E

            IReadOnlyList<IReadOnlyCell> IReadOnlyCell.Adjacent => Adjacent;
        }
        #endregion

        float _cellSize;
        Cell[,] _grid;

        public IReadOnlyCell this[Vector2Int index] => _grid[index.y, index.x];

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

        // �אڃ��X�g���M�Y���ɕ`��
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
        /// �����O���t�Ƃ���2�̃Z�����m���q����
        /// </summary>
        public void Connect(Vector2Int a, Vector2Int b)
        {
            if (!Utility.CheckInLength(_grid, a)) return;
            if (!Utility.CheckInLength(_grid, b)) return;

            _grid[a.y, a.x].Adjacent.Add(_grid[b.y, b.x]);
            _grid[b.y, b.x].Adjacent.Add(_grid[a.y, a.x]);
        }

        /// <summary>
        /// 2�̃Z�����m���ڑ�����Ă��邩
        /// </summary>
        public bool IsConnected(Vector2Int a, Vector2Int b)
        {
            if (!Utility.CheckInLength(_grid, a)) return false;
            if (!Utility.CheckInLength(_grid, b)) return false;

            // �����O���t�Ȃ̂ŕЕ����ׂ�΂悢
            return _grid[a.y, a.x].Adjacent.Contains(_grid[b.y, b.x]);
        }

        /// <summary>
        /// �Z���̃��P�[�V�������Z�b�g
        /// </summary>
        public void SetLocation(LocationKey key, Vector2Int index, ILocation location)
        {
            if (!Utility.CheckInLength(_grid, index)) return;

            _grid[index.y, index.x].LocationKey = key;
            _grid[index.y, index.x].Location = location;
        }

        /// <summary>
        /// �Z���ɂ���A�C�e�����Z�b�g
        /// </summary>
        public void SetItem(ItemKey key, Vector2Int index, IItem item)
        {
            if (!Utility.CheckInLength(_grid, index)) return;

            _grid[index.y, index.x].ItemKey = key;
            _grid[index.y, index.x].Item = item;
        }

        /// <summary>
        /// �Z���ɂ���L�����N�^�[���Z�b�g
        /// </summary>
        public void SetCharacter(CharacterKey key, Vector2Int index, ICharacter character)
        {
            if (!Utility.CheckInLength(_grid, index)) return;

            _grid[index.y, index.x].CharacterKey = key;
            _grid[index.y, index.x].Character = character;
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
