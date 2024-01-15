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
        // �v���C���[�̏��
        public Player.Forward Forward { get; set; }
        public bool OnStageBorder { get; set; }
        public bool OnHoleFront { get; set; }
        public bool OnStepFront { get; set; }
        // ���W�b�N�̏��
        public Direction GoalDirection { get; set; }
    }
}
