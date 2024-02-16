using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class MeshDrawer : MonoBehaviour
    {
        // from�̈ʒu��from����to�ւ̃x�N�g�������Ƃ������E�̒��_
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
        [Header("���̕�")]
        [SerializeField] float _width = 0.1f;
        [Header("�M�Y���ɕ`��")]
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
        /// �}�E�X�J�[�\���̈ʒu�𒸓_�Ƃ��ĕۑ����A�O�Ղ̃��b�V�����쐬�B
        /// </summary>
        public void MouseTrace(float space)
        {
            Plane plane = new(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float depth))
            {
                Vector3 pos = ray.origin + ray.direction * depth;
                if (_points.Count == 0) _points.Add(pos);

                // 1��̃}�E�X�̈ړ��œ_���m�̋�����2�{�ȏ㓮���\��������
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
        /// ����`�悷�郁�b�V�������B
        /// </summary>
        public void Line(IReadOnlyList<Vector3> points, Vector3 axis)
        {
            // ���g���R�s�[
            _points.Clear();
            _points.AddRange(points);
            
            for (int i = 1; i < _points.Count; i++)
            {
                _sections.Add(new(_points[i - 1], _points[i], axis, _width));
            }

            Create(axis);
        }

        // �e���_�Ƃ��̍��E�̒��_����Ƀ��b�V�����쐬
        void Create(Vector3 axis)
        {
            if (_points.Count < 2 || _sections.Count < 1) return;

            // ���[�̒��_�܂ł�`�悷�邽�߂ɂ�����g�A���E�̒��_��ǉ��B
            // �O������1�O�̒��_�Ɠ����ɂ��Ă����B
            Vector3 forward = _points[^1] + _points[^1] - _points[^2];
            _sections.Add(new(_points[^1], forward, axis, _width));

            // 4�̒��_�ŋ�`�����B
            Vector3[] vertices = new Vector3[(_sections.Count - 1) * 4];
            Vector2[] uvs = new Vector2[(_sections.Count - 1) * 4];
            for (int i = 0; i < _sections.Count - 1; i++)
            {
                // 2�̒��_�̍��E�̒��_����g�ݍ��킹��B
                vertices[i * 4] = _sections[i + 1].Left;
                vertices[i * 4 + 1] = _sections[i + 1].Right;
                vertices[i * 4 + 2] = _sections[i].Left;
                vertices[i * 4 + 3] = _sections[i].Right;

                uvs[i * 4] = new Vector2(0, 1);
                uvs[i * 4 + 1] = new Vector2(0, 0);
                uvs[i * 4 + 2] = new Vector2(1, 1);
                uvs[i * 4 + 3] = new Vector2(1, 0);
            }

            // 3�̒��_�̎O�p�`��2�g�ݍ��킹��B
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

            // ���[�̒��܂ł�`�悷�邽�߂ɒǉ��������_���폜���Ă����B
            _sections.RemoveAt(_sections.Count - 1);
        }

        void OnDrawGizmos()
        {
            if (_drawOnGizmos) DrawOnGizmos();
        }

        // �M�Y���ɕ`��
        void DrawOnGizmos()
        {
            if (_points == null || _sections == null) return;

            // ���S�_�ƌq����
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

            // ���E�̒f��
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
