using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public interface IReadOnlyGameState
    {
        public Player.Forward Forward { get; }
        public bool OnStageBorder { get; }
        public bool OnHoleFront { get; }
        public bool OnStepFront { get; }
        public Direction GoalDirection { get; }
    }
}