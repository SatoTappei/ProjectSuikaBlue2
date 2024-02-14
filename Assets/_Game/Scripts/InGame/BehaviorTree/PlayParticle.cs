using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// 子ノードの実行中に一定間隔でパーティクルを再生するデコレータノード。
    /// </summary>
    public class PlayParticle : Node
    {
        readonly Enemy _self;
        readonly Node _child;
        readonly Enemy.ParticleKey _key;
        readonly float _interval;

        float _elapsed;

        public PlayParticle(Enemy self, Enemy.ParticleKey key,
            float interval, Node node, string name = nameof(PlayParticle)) : base(name)
        {
            _self = self;
            _child = node;
            _key = key;
            _interval = interval;
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
            if(_elapsed > _interval)
            {
                _elapsed = 0;
                _self.PlayParticle(_key);
            }

            return _child.Update();
        }
    }
}