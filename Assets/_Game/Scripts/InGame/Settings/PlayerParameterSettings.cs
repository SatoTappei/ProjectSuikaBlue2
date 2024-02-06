using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "PlayerParameterSettings_")]
    public class PlayerParameterSettings : ScriptableObject
    {
        [Header("�n�ʂ���̍���")]
        [SerializeField] float _groundOffset = 1.0f;
        [Header("�ړ����x")]
        [SerializeField] float _moveSpeed = 0.5f;
        [Header("�U��������x")]
        [SerializeField] float _rotateSpeed = 0.5f;

        /// <summary>
        /// �n�ʂ���̍���
        /// </summary>
        public Vector3 GroundOffset => Vector3.up * _groundOffset;
        /// <summary>
        /// �ړ����x
        /// </summary>
        public float MoveSpeed => _moveSpeed;
        /// <summary>
        /// �U��������x
        /// </summary>
        public float RotateSpeed => _rotateSpeed;
    }
}
