using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "CharacterSettings_")]
    public class CharacterSettings : ScriptableObject
    {
        [Header("�L�����N�^�[�̖��O")]
        [SerializeField] string _characterName = "�߂���";
        [Header("�S��")]
        [SerializeField] int _maxMental = 100;
        [SerializeField] int _minMental = 0;
        [SerializeField] int _defaultMental = 100;
        [Header("�v���C���[�ɕԓ��̗D��x")]
        [SerializeField] int _playerPriority = 100;
        [Header("�e��C�x���g�Ƀ��A�N�V�����̗D��x")]
        [SerializeField] int _inGameEventPriority = 1000;
        [Header("��_���[�W�Ƀ��A�N�V�����̗D��x")]
        [SerializeField] int _damagedPriority = 500;

        /// <summary>
        /// ��b����p�̃w�b�_�[
        /// </summary>
        public string LogHeader => $"{_characterName}: ";
        /// <summary>
        /// �S��̍ő�l
        /// </summary>
        public int MaxMental => _maxMental;
        /// <summary>
        /// �S��̍ŏ��l
        /// </summary>
        public int MinMental => _minMental;
        /// <summary>
        /// �S��̏����l
        /// </summary>
        public int DefaultMental => Mathf.Clamp(_defaultMental, _minMental, _maxMental);
        /// <summary>
        /// �v���C���[�ɕԓ��̗D��x
        /// </summary>
        public int PlayerPriority => _playerPriority;
        /// <summary>
        /// �e��C�x���g�Ƀ��A�N�V�����̗D��x
        /// </summary>
        public int InGameEventPriority => _inGameEventPriority;
        /// <summary>
        /// �_���[�W���󂯂��ۂɃ��A�N�V�����̗D��x
        /// </summary>
        public int DamagedPriority => _damagedPriority;
    }
}
