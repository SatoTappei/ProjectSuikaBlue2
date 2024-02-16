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
        /// 開始位置
        /// </summary>
        public Vector2Int StartIndex { get; set; }
        /// <summary>
        /// 現在位置
        /// </summary>
        public Vector2Int PlayerIndex { get; set; }
        /// <summary>
        /// 宝を入手済み
        /// </summary>
        public bool IsGetTreasure { get; set; }
        /// <summary>
        /// 入口にいる
        /// </summary>
        public bool IsStandingEntrance => PlayerIndex == StartIndex;
        /// <summary>
        /// 前方向へ移動の期待値
        /// </summary>
        public int ForwardEvaluate { get; set; }
        /// <summary>
        /// 後ろ方向へ移動の期待値
        /// </summary>
        public int BackEvaluate { get; set; }
        /// <summary>
        /// 左方向へ移動の期待値
        /// </summary>
        public int LeftEvaluate { get; set; }
        /// <summary>
        /// 右方向へ移動の期待値
        /// </summary>
        public int RightEvaluate { get; set; }
    }
}
