using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// プレイヤーの方向を向き続けるアクションノード
    /// </summary>
    public class LookAtPlayer : Node
    {
        readonly Enemy _self;
        readonly float _speed;
        readonly float _duration;

        float _elapsed = 0;

        public LookAtPlayer(float speed, float duration, Enemy self, string name = nameof(LookAtPlayer)) : base(name)
        {
            _self = self;
            _speed = speed;
            _duration = duration;
        }

        protected override void OnBreak()
        {
            _elapsed = 0;
        }

        protected override void Enter()
        {
            _elapsed = 0;
        }

        protected override void Exit()
        {
            _elapsed = 0;
        }

        protected override State Stay()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed > _duration) return State.Success;
            else
            {
                _self.RotateToPlayer(Time.deltaTime * _speed);
                return State.Running;
            }
        }
    }
}
