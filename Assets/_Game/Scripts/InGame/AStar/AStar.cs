using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// A*アルゴリズムを用いてグラフ上の経路探索を行う。
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
            // 位置、添え字、隣接リスト等の情報はそのまま
            public IReadOnlyCell Cell { get; private set; }
            // 二分ヒープで管理
            public int BinaryHeapIndex { get; set; }

            // 総コストが同じ場合、推定コストで比較
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
        /// コンストラクタで渡した二次元配列の添え字(座標)を2つ指定して経路を求める。
        /// </summary>
        public List<IReadOnlyCell> Pathfinding(Vector2Int start, Vector2Int goal)
        {
            Reset();

            // スタートからゴールまでのヒューリスティックコストを計算
            Node current = _graph[start.y, start.x];
            current.H = Heuristic(start, goal);

            // OpenとCloseを管理
            BinaryHeap<Node> open = new(_graph.Length);
            open.Push(current);
            HashSet<Node> close = new();

            while (open.Count > 0)
            {
                current = open.Pop();
                
                // ゴールにたどり着いた場合は経路を作成して返す
                if (current.Cell.Index == goal) return CreatePath(current);

                close.Add(current);

                // 隣接リスト
                foreach (IReadOnlyCell c in current.Cell.Adjacent)
                {
                    Node neighbour = _graph[c.Index.y, c.Index.x];

                    // Close状態のノードなら弾く
                    if (close.Contains(neighbour)) continue;

                    int g = current.G + Heuristic(current.Cell.Index, neighbour.Cell.Index);
                    int h = Heuristic(neighbour.Cell.Index, goal);
                    int f = g + h;
                    bool unOpened = !open.Contains(neighbour);

                    // 総コストがより低い、もしくはOpen状態ではない場合
                    if (f < neighbour.F || unOpened)
                    {
                        neighbour.G = g;
                        neighbour.H = h;
                        neighbour.Parent = current;
                    }

                    // Open状態に変更
                    if (unOpened) open.Push(neighbour);
                }
            }

            return null;
        }

        // 計算に使用した値をリセット
        public void Reset()
        {
            foreach (Node n in _graph)
            {
                n.Parent = null;
                n.G = int.MaxValue / 2;
                n.H = int.MaxValue / 2;
            }
        }

        // ヒューリスティックコストの計算
        int Heuristic(Vector2Int a, Vector2Int b)
        {
            // 今回はノードの配置がグリッドと同じく上下左右なのでマンハッタン距離が適している。
            bool gridBase = true; 

            if (gridBase)
            {
                // マンハッタン距離
                int x = Mathf.Abs(a.x - b.x);
                int y = Mathf.Abs(a.y - b.y);

                // 斜め移動に対応。
                // 短い方に斜め移動した後、残りを真っ直ぐに移動する。
                if (x > y) return 14 * y + 10 * (x - y);
                else return 14 * x + 10 * (y - x);
            }
            else
            {
                // ユークリッド距離の2乗
                Vector3 p = _graph[a.y, a.x].Cell.Position;
                Vector3 q = _graph[b.y, b.x].Cell.Position;

                return Mathf.CeilToInt(MyMath.SqrMagnitude(p - q));
            }
        }

        // 経路を返す
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
