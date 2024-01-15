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
        // プレイヤーの状態
        public Player.Forward Forward { get; set; }
        public bool OnStageBorder { get; set; }
        public bool OnHoleFront { get; set; }
        public bool OnStepFront { get; set; }
        // ロジックの状態
        public Direction GoalDirection { get; set; }
    }
}
