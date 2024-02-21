using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public interface IReadOnlyGameState
    {
        /// <summary>
        /// �J�n�ʒu
        /// </summary>
        public Vector2Int StartIndex { get; }
        /// <summary>
        /// ���݈ʒu
        /// </summary>
        public Vector2Int PlayerIndex { get; }
        /// <summary>
        /// �C���Q�[�����Ńv���C���[�̑��삪�o����悤�ɂȂ���
        /// </summary>
        public bool IsInGameReady { get; }
        /// <summary>
        /// ������ς�
        /// </summary>
        public bool IsGetTreasure { get; }
        /// <summary>
        /// �����ɂ���
        /// </summary>
        public bool IsStandingEntrance => PlayerIndex == StartIndex;
        /// <summary>
        /// �C���Q�[�����ŃN���A�����𖞂�����
        /// </summary>
        public bool IsInGameClear { get; }
        /// <summary>
        /// �O�����ֈړ��̊��Ғl
        /// </summary>
        public int ForwardEvaluate { get; }
        /// <summary>
        /// �������ֈړ��̊��Ғl
        /// </summary>
        public int BackEvaluate { get; }
        /// <summary>
        /// �������ֈړ��̊��Ғl
        /// </summary>
        public int LeftEvaluate { get; }
        /// <summary>
        /// �E�����ֈړ��̊��Ғl
        /// </summary>
        public int RightEvaluate { get; }
        /// <summary>
        /// �Ō�Ƀ_���[�W���󂯂�����
        /// </summary>
        public float LastDamagedTime { get; set; }
    }
}