using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// A*�A���S���Y����p���ăO���t��̌o�H�T�����s���B
    /// </summary>
    public class AStar
    {
        class Node : IBinaryHeapCollectable<Node>
        {
            public Node(IReadOnlyCell cell)
            {
                Cell = cell;
            }

            public Node Parent { get; set; }
            public int G { get; set; }
            public int H { get; set; }
            public int F => G + H;
            // �ʒu�A�Y�����A�אڃ��X�g���̏��͂��̂܂�
            public IReadOnlyCell Cell { get; private set; }
            // �񕪃q�[�v�ŊǗ�
            public int BinaryHeapIndex { get; set; }

            // ���R�X�g�������ꍇ�A����R�X�g�Ŕ�r
            public int CompareTo(Node other)
            {
                int c = F.CompareTo(other.F);
                if (c == 0) c = H.CompareTo(other.H);

                return c;
            }
        }

        Node[,] _graph;

        public AStar(IReadOnlyCell[,] grid)
        {
            int h = grid.GetLength(0);
            int w = grid.GetLength(1);

            _graph = new Node[h, w];
            for (int i = 0; i < h; i++)
            {
                for (int k = 0; k < w; k++)
                {
                    _graph[i, k] = new(grid[i, k]);
                }
            }
        }

        /// <summary>
        /// �R���X�g���N�^�œn�����񎟌��z��̓Y����(���W)��2�w�肵�Čo�H�����߂�B
        /// </summary>
        public List<IReadOnlyCell> Pathfinding(Vector2Int start, Vector2Int goal)
        {
            Reset();

            // �X�^�[�g����S�[���܂ł̃q���[���X�e�B�b�N�R�X�g���v�Z
            Node current = _graph[start.y, start.x];
            current.H = Heuristic(start, goal);

            // Open��Close���Ǘ�
            BinaryHeap<Node> open = new(_graph.Length);
            open.Push(current);
            HashSet<Node> close = new();

            while (open.Count > 0)
            {
                current = open.Pop();
                
                // �S�[���ɂ��ǂ蒅�����ꍇ�͌o�H���쐬���ĕԂ�
                if (current.Cell.Index == goal) return CreatePath(current);

                close.Add(current);

                // �אڃ��X�g
                foreach (IReadOnlyCell c in current.Cell.Adjacent)
                {
                    Node neighbour = _graph[c.Index.y, c.Index.x];

                    // Close��Ԃ̃m�[�h�Ȃ�e��
                    if (close.Contains(neighbour)) continue;

                    int g = current.G + Heuristic(current.Cell.Index, neighbour.Cell.Index);
                    int h = Heuristic(neighbour.Cell.Index, goal);
                    int f = g + h;
                    bool unOpened = !open.Contains(neighbour);

                    // ���R�X�g�����Ⴂ�A��������Open��Ԃł͂Ȃ��ꍇ
                    if (f < neighbour.F || unOpened)
                    {
                        neighbour.G = g;
                        neighbour.H = h;
                        neighbour.Parent = current;
                    }

                    // Open��ԂɕύX
                    if (unOpened) open.Push(neighbour);
                }
            }

            return null;
        }

        // �v�Z�Ɏg�p�����l�����Z�b�g
        public void Reset()
        {
            foreach (Node n in _graph)
            {
                n.Parent = null;
                n.G = int.MaxValue / 2;
                n.H = int.MaxValue / 2;
            }
        }

        // �q���[���X�e�B�b�N�R�X�g�̌v�Z
        int Heuristic(Vector2Int a, Vector2Int b)
        {
            // ����̓m�[�h�̔z�u���O���b�h�Ɠ������㉺���E�Ȃ̂Ń}���n�b�^���������K���Ă���B
            bool gridBase = true; 

            if (gridBase)
            {
                // �}���n�b�^������
                int x = Mathf.Abs(a.x - b.x);
                int y = Mathf.Abs(a.y - b.y);

                // �΂߈ړ��ɑΉ��B
                // �Z�����Ɏ΂߈ړ�������A�c���^�������Ɉړ�����B
                if (x > y) return 14 * y + 10 * (x - y);
                else return 14 * x + 10 * (y - x);
            }
            else
            {
                // ���[�N���b�h������2��
                Vector3 p = _graph[a.y, a.x].Cell.Position;
                Vector3 q = _graph[b.y, b.x].Cell.Position;

                return Mathf.CeilToInt(MyMath.SqrMagnitude(p - q));
            }
        }

        // �o�H��Ԃ�
        List<IReadOnlyCell> CreatePath(Node node)
        {
            List<IReadOnlyCell> path = new();
            while (node.Parent != null)
            {
                path.Add(node.Cell);
                node = node.Parent;
            }
            
            path.Add(node.Cell);
            path.Reverse();

            return path;
        }
    }
}
