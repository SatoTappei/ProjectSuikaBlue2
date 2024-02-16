using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class MeshDrawer : MonoBehaviour
    {
        // fromの位置にfromからtoへのベクトルを軸とした左右の頂点
        class Section
        {
            public Section(Vector3 from, Vector3 to, Vector3 rhs, float width)
            {
                Vector3 cross = Vector3.Cross(to - from, rhs).normalized * width;
                Left = cross + from; 
                Right = -cross + from;
            }

            public Vector3 Left { get; private set; }
            public Vector3 Right { get; private set; }
        }

        [SerializeField] MeshFilter _filter;
        [SerializeField] MeshRenderer _renderer;
        [SerializeField] MeshCollider _collider;
        [Header("線の幅")]
        [SerializeField] float _width = 0.1f;
        [Header("ギズモに描画")]
        [SerializeField] bool _drawOnGizmos = true;

        Mesh _mesh;
        List<Vector3> _points;
        List<Section> _sections;

        void Awake()
        {
            _mesh = new();
            _mesh.name = "MeshDrawer_LineMesh";
            _points = new();
            _sections = new();
        }

        /// <summary>
        /// マウスカーソルの位置を頂点として保存し、軌跡のメッシュを作成。
        /// </summary>
        public void MouseTrace(float space)
        {
            Plane plane = new(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float depth))
            {
                Vector3 pos = ray.origin + ray.direction * depth;
                if (_points.Count == 0) _points.Add(pos);

                // 1回のマウスの移動で点同士の距離の2倍以上動く可能性がある
                while ((_points[^1] - pos).magnitude >= space)
                {
                    Vector3 last = _points[^1];
                    Vector3 next = _points[^1] + (pos - _points[^1]).normalized * space;

                    _points.Add(next);
                    _sections.Add(new(last, next, Vector3.up, _width));
                }
            }

            Create(Vector3.up);
        }

        /// <summary>
        /// 線を描画するメッシュを作る。
        /// </summary>
        public void Line(IReadOnlyList<Vector3> points, Vector3 axis)
        {
            // 中身をコピー
            _points.Clear();
            _points.AddRange(points);
            
            for (int i = 1; i < _points.Count; i++)
            {
                _sections.Add(new(_points[i - 1], _points[i], axis, _width));
            }

            Create(axis);
        }

        // 各頂点とその左右の頂点を基にメッシュを作成
        void Create(Vector3 axis)
        {
            if (_points.Count < 2 || _sections.Count < 1) return;

            // 末端の頂点までを描画するためにもう一組、左右の頂点を追加。
            // 前方向は1つ前の頂点と同じにしておく。
            Vector3 forward = _points[^1] + _points[^1] - _points[^2];
            _sections.Add(new(_points[^1], forward, axis, _width));

            // 4つの頂点で矩形を作る。
            Vector3[] vertices = new Vector3[(_sections.Count - 1) * 4];
            Vector2[] uvs = new Vector2[(_sections.Count - 1) * 4];
            for (int i = 0; i < _sections.Count - 1; i++)
            {
                // 2つの頂点の左右の頂点から組み合わせる。
                vertices[i * 4] = _sections[i + 1].Left;
                vertices[i * 4 + 1] = _sections[i + 1].Right;
                vertices[i * 4 + 2] = _sections[i].Left;
                vertices[i * 4 + 3] = _sections[i].Right;

                uvs[i * 4] = new Vector2(0, 1);
                uvs[i * 4 + 1] = new Vector2(0, 0);
                uvs[i * 4 + 2] = new Vector2(1, 1);
                uvs[i * 4 + 3] = new Vector2(1, 0);
            }

            // 3つの頂点の三角形を2つ組み合わせる。
            int[] triangles = new int[(_sections.Count - 1) * 6];
            for (int i = 0; i < _sections.Count - 1; i++)
            {
                triangles[i * 6] = i * 4;
                triangles[i * 6 + 1] = i * 4 + 3;
                triangles[i * 6 + 2] = i * 4 + 2;
                triangles[i * 6 + 3] = i * 4;
                triangles[i * 6 + 4] = i * 4 + 1;
                triangles[i * 6 + 5] = i * 4 + 3;
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.uv = uvs;
            _mesh.Optimize();
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();

            _filter.mesh = _mesh;
            if (_collider != null) _collider.sharedMesh = _mesh;

            // 末端の頂までを描画するために追加した頂点を削除しておく。
            _sections.RemoveAt(_sections.Count - 1);
        }

        void OnDrawGizmos()
        {
            if (_drawOnGizmos) DrawOnGizmos();
        }

        // ギズモに描画
        void DrawOnGizmos()
        {
            if (_points == null || _sections == null) return;

            // 中心点と繋ぐ線
            Vector3? prev = null;
            foreach (Vector3 p in _points)
            {
                if (prev != null)
                {
                    Gizmos.DrawLine((Vector3)prev, p);
                }

                Gizmos.DrawSphere(p, 0.2f);
                prev = p;
            }

            // 左右の断面
            foreach (Section sec in _sections)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(sec.Left, 0.2f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(sec.Right, 0.2f);
            }
        }
    }
}
