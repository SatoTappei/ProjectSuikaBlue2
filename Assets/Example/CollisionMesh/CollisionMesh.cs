using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.CollisionMesh
{
    [System.Serializable]
    class CollisionMesh
    {
        [SerializeField] MeshCollider _collider;
        [SerializeField] MeshFilter _filter;
        [SerializeField] float _pieceAngle = 10.0f;

        Mesh _mesh;
        Vector3[] _vertices;
        int[] _triangles;

        /// <summary>
        /// 2�̊p�x�Ԃɔ�������������̃��b�V�����쐬����B
        /// �쐬�������b�V����MeshCollider������GameObject�ɒǏ]����B
        /// </summary>
        public void Create(float startAngle, float endAngle, float radius)
        {
            // �����v���ɂȂ�P�[�X�ł͊J�n�ƏI�������ւ��邱�ƂŎ��v���ɏC������B
            if (startAngle > endAngle)
            {
                (startAngle, endAngle) = (endAngle, startAngle);
            }

            // �S�̂̊p�x���O�p�`������̊p�x�Ŋ��邱�ƂŁA�����O�p�`�����K�v�����邩�����߂�B
            int triangleCount = Mathf.CeilToInt((endAngle - startAngle) / _pieceAngle);
            _vertices = new Vector3[triangleCount + 2];
            _triangles = new int[triangleCount * 3];

            _vertices[0] = Vector3.zero;
            // 0�Ԗڂ̒��_����J�n�p�̒��_�։��т�G�b�W��1�� + �O�p�`�̌��ɉ����Ēǉ�����钸�_�ւ̃G�b�W�B
            for (int i = 0; i <= triangleCount; i++)
            {
                float currentAngle = startAngle + i * _pieceAngle;
                currentAngle = Mathf.Min(currentAngle, endAngle);
                // Y���𒆐S���ɂ��邱�Ƃ�XZ���ʏ�Ɋp�x���L����B
                _vertices[i + 1] = Quaternion.AngleAxis(currentAngle, Vector3.up) * Vector3.forward * radius;
            }

            for (int i = 0; i < triangleCount; i++)
            {
                // ���Ȃ̂łǂ̎O�p�`���ŏ��̒��_��0�Ԗڂ̒��_�ɂȂ�B
                _triangles[i * 3] = 0;
                _triangles[i * 3 + 1] = i + 1;
                _triangles[i * 3 + 2] = i + 2;
            }

            if (_mesh == null)
            {
                _mesh = new();
                _mesh.name = "CollisionMesh";
            }

            _mesh.Clear();
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.Optimize();
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();

            _filter.mesh = _mesh;
            _collider.sharedMesh = _mesh;
        }

        /// <summary>
        /// �M�Y���Ƀ��b�V����`�悷��
        /// </summary>
        public void DebugDraw()
        {
            if (_triangles == null || _vertices == null) return;

            // �d�l:��]������Ɛ���ɕ\������Ȃ�
            Vector3 center = _collider.transform.position;
            for (int i = 0; i < _triangles.Length; i += 3)
            {
                Debug.DrawLine(center + _vertices[_triangles[i]], center + _vertices[_triangles[i + 1]]);
                Debug.DrawLine(center + _vertices[_triangles[i + 1]], center + _vertices[_triangles[i + 2]]);
                Debug.DrawLine(center + _vertices[_triangles[i + 2]], center + _vertices[_triangles[i]]);
            }
        }
    }
}
