using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// キャラクターを検出するアクションノード
    /// </summary>
    public class CharacterDetect : Node
    {
        readonly Enemy _self;
        readonly int _distance;

        public CharacterDetect(int distance, Enemy self, string name = nameof(CharacterDetect)) : base(name)
        {
            _distance = distance;
            _self = self;
        }

        protected override void OnBreak()
        {
        }

        protected override void Enter()
        {           
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // プレイヤーとの距離が一定以下かつ、見えているか
            if (_self.DetectPlayer(_distance, checkWithinSight: true))
            {
                return State.Success;
            }
            else return State.Failure;
        }
    }
}
