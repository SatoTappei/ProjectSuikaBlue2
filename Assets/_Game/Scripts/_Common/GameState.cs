using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// InGame側が値を書き込み、Character側が読み取って次の判断をする。
    /// </summary>
    public class GameState : IReadOnlyGameState
    {
        /// <summary>
        /// プレイヤーの前方
        /// </summary>
        public GameExample.Player.Forward Forward { get; set; }
        /// <summary>
        /// ステージの端に立っているかを判定
        /// </summary>
        public bool OnStageBorder { get; set; }
        /// <summary>
        /// 目の前に飛び越えられる穴があるかを判定
        /// </summary>
        public bool OnHoleFront { get; set; }
        /// <summary>
        /// 目の前に飛び越えられる段差があるかを判定
        /// </summary>
        public bool OnStepFront { get; set; }
        /// <summary>
        /// プレイヤーから見たゴールの方角
        /// </summary>
        //public EightDirection GoalDirection { get; set; }
    }
}
