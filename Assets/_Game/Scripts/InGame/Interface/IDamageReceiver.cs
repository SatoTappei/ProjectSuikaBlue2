using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// �_���[�W���󂯂�I�u�W�F�N�g�Ɏ���
    /// </summary>
    public interface IDamageReceiver
    {
        /// <summary>
        /// �����̒l�̃_���[�W���󂯂�
        /// </summary>
        public void Damage();
    }
}
