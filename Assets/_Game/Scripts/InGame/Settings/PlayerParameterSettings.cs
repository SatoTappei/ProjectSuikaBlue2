using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "PlayerParameterSettings_")]
    public class PlayerParameterSettings : ScriptableObject
    {
        [Header("地面からの高さ")]
        [SerializeField] float _groundOffset = 1.0f;
        [Header("移動速度")]
        [SerializeField] float _moveSpeed = 0.5f;
        [Header("振り向き速度")]
        [SerializeField] float _rotateSpeed = 0.5f;

        /// <summary>
        /// 地面からの高さ
        /// </summary>
        public Vector3 GroundOffset => Vector3.up * _groundOffset;
        /// <summary>
        /// 移動速度
        /// </summary>
        public float MoveSpeed => _moveSpeed;
        /// <summary>
        /// 振り向き速度
        /// </summary>
        public float RotateSpeed => _rotateSpeed;
    }
}
