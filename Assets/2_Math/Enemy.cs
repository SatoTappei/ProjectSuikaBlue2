using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.CrossProduct
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] MeshRenderer _renderer;
        [SerializeField] Material _defaultMat;
        [SerializeField] Material _changedMat;
        [SerializeField] Transform _player;
        [SerializeField] bool _isDrawGizmos = true;

        void Start()
        {
            _renderer.material = _defaultMat;
        }

        void Update()
        {
            Vector3 p = _player.forward;
            Vector3 e = (transform.position - _player.position).normalized;
            Vector3 cross = MyMath.Cross(p, e);

            Debug.Log(cross.y >= 0 ? "‰E‘¤‚É‚¢‚é" : "¶‘¤‚É‚¢‚é");

            _renderer.material = cross.y >= 0 ? _defaultMat : _changedMat;

            if (_isDrawGizmos)
            {
                Debug.DrawRay(_player.position, p * 3.0f, Color.green);
                Debug.DrawRay(_player.position, e * 3.0f, Color.green);
                Debug.DrawRay(_player.position, cross * 3.0f, Color.green);
            }
        }
    }
}
