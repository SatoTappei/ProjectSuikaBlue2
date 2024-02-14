using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// 一定時間待つアクションノード。
    /// </summary>
    public class WaitForTime : Node
    {
        readonly float _duration;

        float _elapsed;

        public WaitForTime(float duration, string name = nameof(WaitForTime)) : base(name)
        {
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
            // 一定時間経過後に成功を、それまでは実行中を返す
            if (_elapsed > _duration) return State.Success;
            else return State.Running;
        }
    }
}
