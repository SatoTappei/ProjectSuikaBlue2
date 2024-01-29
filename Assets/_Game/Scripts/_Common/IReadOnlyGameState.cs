using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public interface IReadOnlyGameState
    {
        /// <summary>
        /// �v���C���[�̑O��
        /// </summary>
        public GameExample.Player.Forward Forward { get; }
        /// <summary>
        /// �X�e�[�W�̒[�ɗ����Ă��邩�𔻒�
        /// </summary>
        public bool OnStageBorder { get; }
        /// <summary>
        /// �ڂ̑O�ɔ�щz�����錊�����邩�𔻒�
        /// </summary>
        public bool OnHoleFront { get; }
        /// <summary>
        /// �ڂ̑O�ɔ�щz������i�������邩�𔻒�
        /// </summary>
        public bool OnStepFront { get; }
        /// <summary>
        /// �v���C���[���猩���S�[���̕��p
        /// </summary>
        public EightDirection GoalDirection { get; }
    }
}