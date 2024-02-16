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
        /// ������ς�
        /// </summary>
        public bool IsGetTreasure { get; }
        /// <summary>
        /// �����ɂ���
        /// </summary>
        public bool IsStandingEntrance => PlayerIndex == StartIndex;
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
    }
}