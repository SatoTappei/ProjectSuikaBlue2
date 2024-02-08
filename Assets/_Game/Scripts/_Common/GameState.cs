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
        /// �J�n�ʒu
        /// </summary>
        public Vector2Int StartIndex { get; set; }
        /// <summary>
        /// ���݈ʒu
        /// </summary>
        public Vector2Int CurrentIndex { get; set; }
        /// <summary>
        /// ������ς�
        /// </summary>
        public bool IsGetTreasure { get; set; }
        /// <summary>
        /// �����ɂ���
        /// </summary>
        public bool IsStandingAtEntrance => CurrentIndex == StartIndex;
    }
}
