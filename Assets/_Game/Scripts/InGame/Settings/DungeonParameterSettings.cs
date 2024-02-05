using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "DungeonParameterSettings_")]
    public class DungeonParameterSettings : ScriptableObject
    {
        [Header("�_���W���������ݒ�")]
        [SerializeField] int _size = 20;
        [SerializeField] float _cellSize = 1;
        [SerializeField] Vector2Int _entry;
        [Header("WFC�ݒ�")]
        [Min(1)]
        [SerializeField] uint _wfcSeed = 1;
        [SerializeField] bool _wfcRandomSeed;
        [Header("���ȉ���E�H�[�N�ݒ�")]
        [Min(1)]
        [SerializeField] uint _walkSeed = 1;
        [SerializeField] bool _walkRandomSeed;
        [Header("�Z���Ԃ̃��C�L���X�g�̐ݒ�")]
        [SerializeField] Vector3 _cellRayOffset = Vector3.up;
        [SerializeField] int _cellRayHitMax = 9;
        [Header("�X�V���x")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        /// <summary>
        /// �_���W������1�ӂ̃Z����
        /// </summary>
        public int Size => _size;
        /// <summary>
        /// �Z���̑傫��
        /// </summary>
        public float CellSize => _cellSize;
        /// <summary>
        /// �_���W�����̓����̈ʒu
        /// </summary>
        public Vector2Int Entry => _entry;
        /// <summary>
        /// WFC�A���S���Y���̃V�[�h�l
        /// </summary>
        public uint WfcSeed => _wfcSeed;
        /// <summary>
        /// WFC�A���S���Y���Ń����_���ȃV�[�h�l���g�����ǂ���
        /// </summary>
        public bool WfcRandomSeed => _wfcRandomSeed;
        /// <summary>
        /// ���ȉ���ړ��A���S���Y���̃V�[�h�l
        /// </summary>
        public uint WalkSeed => _walkSeed;
        /// <summary>
        /// ���ȉ���ړ��A���S���Y���Ń����_���ȃV�[�h�l���g�����ǂ���
        /// </summary>
        public bool WalkRandomSeed => _walkRandomSeed;
        /// <summary>
        /// �Z���Ԃ̃��C�L���X�g���s���ۂ̃Z���̒��S����̃I�t�Z�b�g
        /// </summary>
        public Vector3 CellRayOffset => _cellRayOffset;
        /// <summary>
        /// �Z���Ԃ̃��C�L���X�g�����肷��ő��
        /// </summary>
        public int CellRayHitMax => _cellRayHitMax;
        /// <summary>
        /// �A���S���Y���̍X�V���x
        /// </summary>
        public float StepSpeed => _stepSpeed;
    }
}
