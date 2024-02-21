using PSB.Game.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    [System.Serializable]
    public class AttackState : State
    {
        [Header("�U���A�j���[�V�����̍Đ�����")]
        [SerializeField] float _animationPlayTime = 1.0f;
        [Header("�U�������܂ł̃f�B���C")]
        [SerializeField] float _attackDelay = 0.5f;
        [Header("�U���͈�(�Z���P��)")]
        [SerializeField] int _range;
        [Header("�U���O�Ƀv���C���[�Ɍ������x")]
        [SerializeField] float _lookSpeed = 10.0f;
        [Header("�U���O�Ƀv���C���[����������")]
        [SerializeField] float _lookDuration = 0.5f;

        Enemy _self;
        Sequence _sequence;

        public override StateKey Key => StateKey.Attack;

        protected override void Init(GameObject self)
        {
            _self = self.GetComponent<Enemy>();

            // �A�j���[�V�����ɍ��킹�čU�������B
            _sequence = new(nameof(Sequence),
                new CharacterDetect(_range, _self),
                new LookAtPlayer(_lookSpeed, _lookDuration, _self), // �U���O�Ƀv���C���[�Ɍ������鎞�Ԃ��K�v
                new PlayAnimation(_self, Enemy.AnimationKey.Kick, _attackDelay),
                new Attack(_self, _range),
                new WaitForTime(_animationPlayTime - _attackDelay)); // �A�j���[�V�����̍Đ������܂ő҂B
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
            // �U���̃A�j���[�V�����Ə��������������ꍇ�͐�����Ԃ��B
            // �͈͓��Ƀv���C���[�����m�ł��Ȃ������ꍇ�͎��s��Ԃ��B
            // �ǂ���ɂ���Idle�ɑJ�ڂ���B
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
