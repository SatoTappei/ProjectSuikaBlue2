using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// �C���X�y�N�^�[��ŕ����̃I�u�W�F�N�g�ɑ΂��āA�����l�����蓖�Ă�K�v��������̂�萔�ŊǗ�����B
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

        const string FootingLayerName = "Footing";
        const string HoleRangeLayerName = "HoleRange";
        const string StageBorderLayerName = "StageBorder";

        /// <summary>
        /// �ڒn����ƃv���C���[���ēx�W�����v�\�ɂȂ郌�C���[
        /// </summary>
        public static int FootingLayer => 1 << LayerMask.NameToLayer(FootingLayerName);
        /// <summary>
        /// ���̔���̃��C���[
        /// </summary>
        public static int HoleRangeLayer => 1 << LayerMask.NameToLayer(HoleRangeLayerName);
        /// <summary>
        /// �X�e�[�W�̒[�𔻒肷�郌�C���[
        /// </summary>
        public static int StageBorderLayer => 1 << LayerMask.NameToLayer(StageBorderLayerName);
    }
}
