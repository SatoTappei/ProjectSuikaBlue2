using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Buffers;
using Random = Unity.Mathematics.Random;

namespace PSB.Game.SAW
{
    public class SelfAvoidingWalk
    {
        public class Cell
        {
            public Cell(int x, int y)
            {
                Index = new Vector2Int(x, y);
                Visited = false;
            }

            public Vector2Int Index { get; private set; }
            public bool Visited { get; set; }
        }

        const float GizmosDrawSize = 0.7f;

        Stack<Cell> _path = new();
        Cell[,] _grid;
        Random _random;
        Vector2Int _start;
        Vector2Int _current;

        public SelfAvoidingWalk(int width, int height, uint seed)
        {
            _random = new Random(seed);
            Create(height, width);
        }

        /// <summary>
        /// �O���b�h�̓񎟌��z��̒���
        /// </summary>
        public int GridLength => _grid.Length;
        /// <summary>
        /// ���݂̍��W
        /// </summary>
        public Vector2Int Current => _current;
        /// <summary>
        /// �X�^�[�g���猻�݂̍��W�܂ł̌o�H
        /// </summary>
        public IReadOnlyCollection<Cell> Path => _path;

        // �O���b�h�𐶐�
        void Create(int height, int width)
        {
            _grid = new Cell[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // ������(0, 0)�ɂȂ��Ă���Axz���ʏ��z��������y�Ax��������x�ɂ��Ă���B
                    // Unity��xz���ʂ̍��W�n�Ɠ����B
                    _grid[y, x] = new Cell(x, y);
                }
            }
        }

        /// <summary>
        /// �J�n���W���Z�b�g
        /// </summary>
        public void SetStartPoint(int y, int x)
        {
            _current.y = y;
            _current.x = x;
            _start = _current;
            _grid[y, x].Visited = true;
            _path.Push(_grid[y,x]);
        }

        /// <summary>
        /// 1�񕪐i�߂āA����ȏ�i�߂邩�ǂ�����Ԃ��B
        /// </summary>
        public bool Step()
        {
            foreach(Vector2Int v in RandomDirection())
            {
                int dy = _current.y + v.y;
                int dx = _current.x + v.x;

                if (!CheckInLength(dy, dx) || _grid[dy, dx].Visited) continue;

                _grid[dy, dx].Visited = true;
                _current = new Vector2Int(dx, dy);
                _path.Push(_grid[dy, dx]);

                return true;
            }

            return false;
        }

        // �㉺���E�������_���ȏ��ԂŕԂ�
        IEnumerable<Vector2Int> RandomDirection()
        {
            Vector2Int[] a = ArrayPool<Vector2Int>.Shared.Rent(4);
            a[0] = Vector2Int.up;
            a[1] = Vector2Int.down;
            a[2] = Vector2Int.left;
            a[3] = Vector2Int.right;

            int r = _random.NextInt(0, 4);
            for (int i = 0; i < 4; i++)
            {
                yield return a[r];
                r++;
                r %= 4;
            }

            Array.Clear(a, 0, a.Length);
            ArrayPool<Vector2Int>.Shared.Return(a);
        }

        bool CheckInLength(int y, int x)
        {
            return 0 <= y && y < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1);
        }

        /// <summary>
        /// �O���b�h���M�Y���ɕ`��
        /// </summary>
        public void DrawGridOnGizmos(float scale)
        {
            if (_grid == null) return;

            for (int y = 0; y < _grid.GetLength(0); y++)
            {
                for (int x = 0; x < _grid.GetLength(1); x++)
                {
                    Gizmos.color = _grid[y, x].Visited ? Color.red : Color.white;
                    Gizmos.DrawSphere(Dungeon.IndexToPosition(x, y, scale), GizmosDrawSize);
                }
            }

            DrawStartAndGoalOnGizmos(scale);
        }

        // �X�^�[�g�ƃS�[���̈ʒu���M�Y���ɕ`��
        void DrawStartAndGoalOnGizmos(float scale)
        {
            if (_path.Count == 0) return;

            // �X�^�[�g�ʒu
            Gizmos.color = Color.green;
            Vector3 s = Dungeon.IndexToPosition(_grid[_start.y, _start.x].Index, scale);
            Gizmos.DrawSphere(s, GizmosDrawSize);
            // �S�[���ʒu
            Gizmos.color = Color.blue;
            Vector3 g = Dungeon.IndexToPosition(_path.Peek().Index, scale);
            Gizmos.DrawSphere(g, GizmosDrawSize);
        }
    }
}
