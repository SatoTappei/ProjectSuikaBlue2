using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public enum Direction
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
