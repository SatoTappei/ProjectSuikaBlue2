using PSB.Game.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    [System.Serializable]
    public class AttackState : State
    {
        [Header("攻撃アニメーションの再生時間")]
        [SerializeField] float _animationPlayTime = 1.0f;
        [Header("攻撃処理までのディレイ")]
        [SerializeField] float _attackDelay = 0.5f;
        [Header("攻撃範囲(セル単位)")]
        [SerializeField] int _range;
        [Header("攻撃前にプレイヤーに向く速度")]
        [SerializeField] float _lookSpeed = 10.0f;
        [Header("攻撃前にプレイヤーを向く時間")]
        [SerializeField] float _lookDuration = 0.5f;

        Enemy _self;
        Sequence _sequence;

        public override StateKey Key => StateKey.Attack;

        protected override void Init(GameObject self)
        {
            _self = self.GetComponent<Enemy>();

            // アニメーションに合わせて攻撃処理。
            _sequence = new(nameof(Sequence),
                new CharacterDetect(_range, _self),
                new LookAtPlayer(_lookSpeed, _lookDuration, _self), // 攻撃前にプレイヤーに向き直る時間が必要
                new PlayAnimation(_self, Enemy.AnimationKey.Kick, _attackDelay),
                new Attack(_self, _range),
                new WaitForTime(_animationPlayTime - _attackDelay)); // アニメーションの再生完了まで待つ。
        }

        protected override void Enter()
        {
            _sequence.Break();
        }

        protected override void Exit()
        {
            _sequence.Break();
        }

        protected override void Stay()
        {
            // 攻撃のアニメーションと処理が完了した場合は成功を返す。
            // 範囲内にプレイヤーを検知できなかった場合は失敗を返す。
            // どちらにしろIdleに遷移する。
            if (_sequence.Update() != Node.State.Running)
            {
                if (_self.TryGetState(StateKey.Idle, out State next))
                {
                    if (TryChangeState(next)) return;
                }
            }
        }
    }
}
