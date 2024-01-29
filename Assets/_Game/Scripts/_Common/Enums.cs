using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    // �����̃N���X�Ŏg�p�����񋓌^�͂����ɋL�q����B

    /// <summary>
    /// Z���̐��̕�����k�AX���̐��̕����𓌂Ƃ���B
    /// </summary>
    public enum Direction
    {
        East,
        West,
        North,
        South,
    }

    public enum EightDirection
    {
        Up,
        Down,
        Left,
        Right,
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight
    }

    public enum AudioKey
    {
        PlayerSendSE, // �v���C���[�����M����
        CharacterSendSE, // �L�����N�^�[�����M����
        TabOpenCloseSE, // �`���b�g�E�B���h�E�̊J��
        CharacterTouchSE, // �L�����N�^�[���^�b�`����
    }

    public enum ParticleKey
    {
        MouseClick, // ��ʂ��N���b�N
        SimpleNotice, // �V���v����ԂŃL�����N�^�[������
    }
}
