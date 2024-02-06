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
        [Header("歩く際になる音の設定")]
        [SerializeField] int _walkSeLoop = 2;
        [SerializeField] float _walkSeDelay = 0.25f;

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
        /// <summary>
        /// 歩く際に鳴る音の回数
        /// </summary>
        public int WalkSeLoop => _walkSeLoop;
        /// <summary>
        /// 歩く際に鳴る音のディレイ
        /// </summary>
        public float WalkSeDelay => _walkSeDelay;
    }
}
