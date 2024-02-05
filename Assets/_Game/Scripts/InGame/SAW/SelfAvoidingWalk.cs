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
        public class Cell : IReadOnlyPosition
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

        const float GizmosDrawSize = 0.7f;

        Stack<Cell> _path = new();
        Cell[,] _grid;
        Random _random;
        Vector2Int _start;
        Vector2Int _current;

        public SelfAvoidingWalk(int width, int height, float cellSize, uint seed)
        {
            _random = new Random(seed);
            Create(height, width, cellSize);
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
        void Create(int height, int width, float cellSize)
        {
            _grid = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    float px = i * cellSize + cellSize / 2;
                    float py = k * cellSize + cellSize / 2;
                    // ������(0, 0)�ɂȂ��Ă���Axz���ʏ��z��������y�Ax��������x�ɂ��Ă���B
                    // Unity��xz���ʂ̍��W�n�Ɠ����B
                    _grid[i, k] = new(i, k, new Vector3(py, 0, px));
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
        public void DrawGridOnGizmos()
        {
            if (_grid == null) return;

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    Gizmos.color = _grid[i, k].Visited ? Color.red : Color.white;
                    Gizmos.DrawSphere(_grid[i, k].Position, GizmosDrawSize);
                }
            }

            DrawStartAndGoalOnGizmos();
        }

        // �X�^�[�g�ƃS�[���̈ʒu���M�Y���ɕ`��
        void DrawStartAndGoalOnGizmos()
        {
            if (_path.Count == 0) return;

            // �X�^�[�g�ʒu
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_grid[_start.y, _start.x].Position, GizmosDrawSize);
            // �S�[���ʒu
            Vector3 gp = _path.Peek().Position;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(gp, GizmosDrawSize);
        }
    }
}
