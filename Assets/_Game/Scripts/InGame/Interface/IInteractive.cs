using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// �L�����N�^�[���C�ӂ̃^�C�~���O�ō�p�\�ȃI�u�W�F�N�g�Ɏ���
    /// </summary>
    public interface IInteractive
    {
        /// <summary>
        /// �K���ȓ������ςȂ��̏���
        /// </summary>
        public void Action(object arg = null);
    }
}
