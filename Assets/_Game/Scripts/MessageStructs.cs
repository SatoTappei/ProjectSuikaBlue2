using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// OpenAPI����̃��X�|���X���L�[���͂ɕϊ�����Character����InGame�ɑ��M�����B
    /// </summary>
    public struct PlayerControlMessage
    {
        public bool KeyDownA;
        public bool KeyDownS;
        public bool KeyDownD;
        public bool KeyDownW;
        public bool KeyDownSpace;
    }
}
