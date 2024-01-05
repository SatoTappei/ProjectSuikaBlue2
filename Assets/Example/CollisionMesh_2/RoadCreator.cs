using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.CollisionMesh
{
    public class Section
    {
        public Vector3 Left;
        public Vector3 Right;
    }

    public class RoadCreator : MonoBehaviour
    {
        [SerializeField] MeshFilter _filter;
        [SerializeField] MeshRenderer _renderer;
        [SerializeField] MeshCollider _collider;
        [Min(1.0f)]
        [SerializeField] float _space = 1.0f;

        Mesh _mesh;
        List<Vector3> _points;
        List<Section> _sections;

        void Awake()
        {
            _mesh = new();
            _mesh.name = "RoadMesh";
            _points = new();
            _sections = new();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() => Input.GetMouseButton(0));
                Add();
                Create();
                await UniTask.WaitForSeconds(0.1f, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        void Add()
        {
            Plane plane = new(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float depth))
            {
                Vector3 pos = ray.origin + ray.direction * depth;
                if (_points.Count == 0) _points.Add(pos);

                // 1‰ñ‚Ìƒ}ƒEƒX‚ÌˆÚ“®‚Å“_“¯Žm‚Ì‹——£‚Ì2”{ˆÈã“®‚­‰Â”\«‚ª‚ ‚é
                while ((_points[^1] - pos).magnitude >= _space)
                {
                    Vector3 last = _points[^1];
                    Vector3 next = _points[^1] + (pos - _points[^1]).normalized * _space;
                    
                    _points.Add(next);
                    _sections.Add(CalculateSection(last, next));
                }
            }
        }

        void Create()
        {
            if (_points.Count < 2 || _sections.Count < 1) return;

            Vector3 forward = _points[^1] + _points[^1] - _points[^2];
            _sections.Add(CalculateSection(_points[^1], forward));

            Vector3[] vertices = new Vector3[(_sections.Count - 1) * 4];
            Vector2[] uvs = new Vector2[(_sections.Count - 1) * 4];
            for (int i = 0; i < _sections.Count - 1; i++)
            {
                vertices[i * 4] = _sections[i + 1].Left;
                vertices[i * 4 + 1] = _sections[i + 1].Right;
                vertices[i * 4 + 2] = _sections[i].Left;
                vertices[i * 4 + 3] = _sections[i].Right;

                uvs[i * 4] = new Vector2(0, 1);
                uvs[i * 4 + 1] = new Vector2(0, 0);
                uvs[i * 4 + 2] = new Vector2(1, 1);
                uvs[i * 4 + 3] = new Vector2(1, 0);
            }

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
            _collider.sharedMesh = _mesh;

            _sections.RemoveAt(_sections.Count - 1);
        }

        Section CalculateSection(Vector3 from, Vector3 to)
        {
            Vector3 cross = Vector3.Cross(to - from, Vector3.up).normalized;
            return new Section() { Left = cross + from, Right = -cross + from };
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // ’†S“_‚ÆŒq‚®ü
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

            // ¶‰E‚Ì’f–Ê
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
