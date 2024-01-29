using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public interface IReadOnlyGameState
    {
        /// <summary>
        /// プレイヤーの前方
        /// </summary>
        public GameExample.Player.Forward Forward { get; }
        /// <summary>
        /// ステージの端に立っているかを判定
        /// </summary>
        public bool OnStageBorder { get; }
        /// <summary>
        /// 目の前に飛び越えられる穴があるかを判定
        /// </summary>
        public bool OnHoleFront { get; }
        /// <summary>
        /// 目の前に飛び越えられる段差があるかを判定
        /// </summary>
        public bool OnStepFront { get; }
        /// <summary>
        /// プレイヤーから見たゴールの方角
        /// </summary>
        public EightDirection GoalDirection { get; }
    }
}