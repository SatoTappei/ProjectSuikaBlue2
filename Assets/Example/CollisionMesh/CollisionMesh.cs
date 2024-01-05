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
        /// 2つの角度間に判定を持った扇状のメッシュを作成する。
        /// 作成したメッシュはMeshColliderを持つGameObjectに追従する。
        /// </summary>
        public void Create(float startAngle, float endAngle, float radius)
        {
            // 反時計回りになるケースでは開始と終了を入れ替えることで時計回りに修正する。
            if (startAngle > endAngle)
            {
                (startAngle, endAngle) = (endAngle, startAngle);
            }

            // 全体の角度を三角形あたりの角度で割ることで、いくつ三角形を作る必要があるかを求める。
            int triangleCount = Mathf.CeilToInt((endAngle - startAngle) / _pieceAngle);
            _vertices = new Vector3[triangleCount + 2];
            _triangles = new int[triangleCount * 3];

            _vertices[0] = Vector3.zero;
            // 0番目の頂点から開始角の頂点へ延びるエッジが1つ + 三角形の個数に応じて追加される頂点へのエッジ。
            for (int i = 0; i <= triangleCount; i++)
            {
                float currentAngle = startAngle + i * _pieceAngle;
                currentAngle = Mathf.Min(currentAngle, endAngle);
                // Y軸を中心軸にすることでXZ平面上に角度が広がる。
                _vertices[i + 1] = Quaternion.AngleAxis(currentAngle, Vector3.up) * Vector3.forward * radius;
            }

            for (int i = 0; i < triangleCount; i++)
            {
                // 扇状なのでどの三角形も最初の頂点は0番目の頂点になる。
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
        /// ギズモにメッシュを描画する
        /// </summary>
        public void DebugDraw()
        {
            if (_triangles == null || _vertices == null) return;

            // 仕様:回転させると正常に表示されない
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
