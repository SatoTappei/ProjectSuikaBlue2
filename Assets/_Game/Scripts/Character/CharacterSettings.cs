using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "CharacterSettings_")]
    public class CharacterSettings : ScriptableObject
    {
        public readonly int MaxMental = 100;
        public readonly int MinMental = 0;

        [Header("�L�����N�^�[�̖��O")]
        [SerializeField] string _characterName = "�߂���";
        [Header("�f�t�H���g�̐S��")]
        [SerializeField] int _defaultMental = 100;
        [Header("�v���C���[�̗D��x")]
        [SerializeField] int _playerPriority = 100;

        /// <summary>
        /// ��b����p�̃w�b�_�[
        /// </summary>
        public string LogHeader => $"{_characterName}: ";
        /// <summary>
        /// �S��̏����l
        /// </summary>
        public int DefaultMental => _defaultMental;
        /// <summary>
        /// �v���C���[�̗D��x
        /// </summary>
        public int PlayerPriority => _playerPriority;
    }
}
