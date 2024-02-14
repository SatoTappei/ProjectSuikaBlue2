using PSB.Game.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    [System.Serializable]
    public class IdleState : State
    {
        [Header("Searchに遷移するまでの待機時間")]
        [SerializeField] float _waitDuration;
        [Header("プレイヤーを検知する距離")]
        [SerializeField] int _detectDistance;

        Enemy _self;
        Sequence _waitForTimeSequence;
        CharacterDetect _characterDetect;

        public override StateKey Key => StateKey.Idle;

        protected override void Init(GameObject self)
        {
            _self = self.GetComponent<Enemy>();

            // 処理はビヘイビアツリーのノードが行うという設計上の理由で
            // 最初にアニメーションを再生するためだけにSequenceにしている。
            _waitForTimeSequence = new(nameof(Sequence),
                new PlayAnimation(_self, Enemy.AnimationKey.Walk, 0),
                new WaitForTime(_waitDuration));

            _characterDetect = new(_detectDistance, _self);
        }

        protected override void Enter()
        {
            _waitForTimeSequence.Break();
            _characterDetect.Break();
        }

        protected override void Exit()
        {
            _waitForTimeSequence.Break();
            _characterDetect.Break();
        }

        protected override void Stay()
        {
            // 一定時間後にSearchに遷移
            // Idleのアニメーション再生はこのシーケンス内なので、こちらを先に処理する。
            if (_waitForTimeSequence.Update() == Node.State.Success)
            {
                if (_self.TryGetState(StateKey.Search, out State next))
                {
                    if (TryChangeState(next)) return;
                }
            }

            // プレイヤーを見つけた場合はChaseに遷移
            if (_characterDetect.Update() == Node.State.Success)
            {
                if (_self.TryGetState(StateKey.Chase, out State next))
                {
                    if (TryChangeState(next)) return;
                }
            }
        }
    }
}