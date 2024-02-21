using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// �A�j���[�V�����̍Đ����s���A�N�V�����m�[�h�B
    /// �Đ�����C�ӂ̎��Ԍo�ߌ�ɐ�����Ԃ��B
    /// </summary>
    public class PlayAnimation : Node
    {
        readonly Enemy _self;
        readonly Enemy.AnimationKey _key;
        readonly float _duration;

        float _elapsed;

        public PlayAnimation(Enemy self, Enemy.AnimationKey key, float duration, 
            string name = nameof(PlayAnimation)) : base(name)
        {
            _self = self;
            _key = key;
            _duration = duration;
        }

        protected override void OnBreak()
        {
            _elapsed = 0;
        }

        protected override void Enter()
        {
            _self.PlayAnimation(_key);
            _elapsed = 0;
        }

        protected override void Exit()
        {
            _elapsed = 0;
        }

        protected override State Stay()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed >= _duration) return State.Success;
            else return State.Running;
        }
    }
}
