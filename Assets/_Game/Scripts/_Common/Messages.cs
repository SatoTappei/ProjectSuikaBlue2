using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    // �����̃N���X�Ŏg�p����郁�b�Z�[�W���O�p�̍\���̂͂����ɋL�q����B
    // �ꊇ�ŊǗ����邽�ߑS�V�[�����ʂŎg���B

    /// <summary>
    /// OpenAI����̃��X�|���X���L�[���͂ɕϊ�����Character����InGame�ɑ��M�����
    /// </summary>
    public struct KeyInputMessage
    {
        public bool KeyDownA;
        public bool KeyDownS;
        public bool KeyDownD;
        public bool KeyDownW;
        public bool KeyDownSpace;
    }
}
