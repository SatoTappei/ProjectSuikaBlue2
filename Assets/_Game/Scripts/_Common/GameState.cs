using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// InGame�����l���������݁ACharacter�����ǂݎ���Ď��̔��f������B
    /// </summary>
    public class GameState : IReadOnlyGameState
    {
        /// <summary>
        /// �v���C���[�̑O��
        /// </summary>
        public GameExample.Player.Forward Forward { get; set; }
        /// <summary>
        /// �X�e�[�W�̒[�ɗ����Ă��邩�𔻒�
        /// </summary>
        public bool OnStageBorder { get; set; }
        /// <summary>
        /// �ڂ̑O�ɔ�щz�����錊�����邩�𔻒�
        /// </summary>
        public bool OnHoleFront { get; set; }
        /// <summary>
        /// �ڂ̑O�ɔ�щz������i�������邩�𔻒�
        /// </summary>
        public bool OnStepFront { get; set; }
        /// <summary>
        /// �v���C���[���猩���S�[���̕��p
        /// </summary>
        //public EightDirection GoalDirection { get; set; }
    }
}
