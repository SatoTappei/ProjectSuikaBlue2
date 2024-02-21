using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public interface IReadOnlyGameState
    {
        /// <summary>
        /// 開始位置
        /// </summary>
        public Vector2Int StartIndex { get; }
        /// <summary>
        /// 現在位置
        /// </summary>
        public Vector2Int PlayerIndex { get; }
        /// <summary>
        /// インゲーム内でプレイヤーの操作が出来るようになった
        /// </summary>
        public bool IsInGameReady { get; }
        /// <summary>
        /// 宝を入手済み
        /// </summary>
        public bool IsGetTreasure { get; }
        /// <summary>
        /// 入口にいる
        /// </summary>
        public bool IsStandingEntrance => PlayerIndex == StartIndex;
        /// <summary>
        /// インゲーム内でクリア条件を満たした
        /// </summary>
        public bool IsInGameClear { get; }
        /// <summary>
        /// 前方向へ移動の期待値
        /// </summary>
        public int ForwardEvaluate { get; }
        /// <summary>
        /// 後ろ方向へ移動の期待値
        /// </summary>
        public int BackEvaluate { get; }
        /// <summary>
        /// 左方向へ移動の期待値
        /// </summary>
        public int LeftEvaluate { get; }
        /// <summary>
        /// 右方向へ移動の期待値
        /// </summary>
        public int RightEvaluate { get; }
        /// <summary>
        /// 最後にダメージを受けた時間
        /// </summary>
        public float LastDamagedTime { get; set; }
    }
}