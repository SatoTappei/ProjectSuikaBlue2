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
        [Header("�����ۂɂȂ鉹�̐ݒ�")]
        [SerializeField] int _walkSeLoop = 2;
        [SerializeField] float _walkSeDelay = 0.25f;

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
        /// <summary>
        /// �����ۂɖ鉹�̉�
        /// </summary>
        public int WalkSeLoop => _walkSeLoop;
        /// <summary>
        /// �����ۂɖ鉹�̃f�B���C
        /// </summary>
        public float WalkSeDelay => _walkSeDelay;
    }
}
