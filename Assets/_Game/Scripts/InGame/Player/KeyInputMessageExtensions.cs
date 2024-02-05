using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class KeyInputMessageExtensions
    {
        /// <summary>
        /// �O��ړ��̃L�[���ǂ����𔻒�B�����̓��͂��������ꍇ�ł��ǂꂩ1�B
        /// </summary>
        public static bool IsMoveKey(this KeyInputMessage msg, out KeyCode key)
        {
            if (msg.KeyDownS) { key = KeyCode.S; return true; }
            else if (msg.KeyDownW) { key = KeyCode.W; return true; }
            else key = default; return false;
        }

        /// <summary>
        /// ���E��]�̃L�[���ǂ����𔻒�B�����̓��͂��������ꍇ�ł��ǂꂩ1�B
        /// </summary>
        public static bool IsRotateKey(this KeyInputMessage msg, out KeyCode key)
        {
            if (msg.KeyDownA) { key = KeyCode.A; return true; }
            else if (msg.KeyDownD) { key = KeyCode.D; return true; }
            else key = default; return false;
        }
    }
}
