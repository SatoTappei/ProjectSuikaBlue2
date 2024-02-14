using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// �C���X�y�N�^�[��ŕ����̃I�u�W�F�N�g�ɑ΂��āA�����l�����蓖�Ă�K�v��������̂�萔�ŊǗ�����B
    /// �ꊇ�ŊǗ����邽�ߑS�V�[�����ʂŎg���B
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// �C���Q�[���̃V�[����
        /// </summary>
        public const string InGameSceneName = "InGame";
        /// <summary>
        /// �L�����N�^�[�Ƃ̉�b�V�[����
        /// </summary>
        public const string CharacterSceneName = "Character3D";
        /// <summary>
        /// �C���Q�[�����̃v���C���[�̃^�O
        /// </summary>
        public const string PlayerTag = "Player";
        /// <summary>
        /// �_���W�����̕ǂȂǔ��肪����I�u�W�F�N�g�̃��C���[
        /// </summary>
        public static readonly int DungeonLayer = 1 << LayerMask.NameToLayer("Dungeon");
    }
}
