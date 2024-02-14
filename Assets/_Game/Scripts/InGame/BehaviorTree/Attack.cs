using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    public class Attack : Node
    {
        readonly Enemy _self;
        readonly int _range;

        public Attack(Enemy self, int range, string name = nameof(Attack)) : base(name)
        {
            _self = self;
            _range = range;
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
            _self.Attack();
            return State.Success;
        }
    }
}
