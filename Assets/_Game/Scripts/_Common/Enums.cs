using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    // �����̃N���X�Ŏg�p�����񋓌^�͂����ɋL�q����B
    // �ꊇ�ŊǗ����邽�ߑS�V�[�����ʂŎg���B

    /// <summary>
    /// �L�����N�^�[�̌����Ă��������\���B
    /// Z���̐��̕�����k�AX���̐��̕����𓌂Ƃ���B
    /// </summary>
    public enum Direction
    {
        East,
        West,
        North,
        South,
    }

    /// <summary>
    /// �^�C�����ǂ̂悤�ȏꏊ�Ȃ̂������߂�L�[
    /// </summary>
    public enum LocationKey
    {
        Normal,
        Start,
        Goal,
    }

    /// <summary>
    /// �A�C�e���𔻒肷�邽�߂̃L�[
    /// </summary>
    public enum ItemKey
    {
        Dummy,
    }

    /// <summary>
    /// �L�����N�^�[�𔻒肷�邽�߂̃L�[
    /// </summary>
    public enum CharacterKey
    {
        Dummy,
    }

    /// <summary>
    /// �Đ����鉹���w�肷�邽�߂̃L�[
    /// </summary>
    public enum AudioKey
    {
        PlayerSendSE, // �v���C���[�����M����
        CharacterSendSE, // �L�����N�^�[�����M����
        TabOpenCloseSE, // �`���b�g�E�B���h�E�̊J��
        CharacterTouchSE, // �L�����N�^�[���^�b�`����
        WalkStepSE,
    }

    /// <summary>
    /// �Đ�����p�[�e�B�N�����w�肷�邽�߂̃L�[
    /// </summary>
    public enum ParticleKey
    {
        MouseClick, // ��ʂ��N���b�N
        SimpleNotice, // �V���v����ԂŃL�����N�^�[������
    }
}
