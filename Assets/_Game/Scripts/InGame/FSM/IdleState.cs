using PSB.Game.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    [System.Serializable]
    public class IdleState : State
    {
        [Header("Search�ɑJ�ڂ���܂ł̑ҋ@����")]
        [SerializeField] float _waitDuration;
        [Header("�v���C���[�����m���鋗��")]
        [SerializeField] int _detectDistance;

        Enemy _self;
        Sequence _waitForTimeSequence;
        CharacterDetect _characterDetect;

        public override StateKey Key => StateKey.Idle;

        protected override void Init(GameObject self)
        {
            _self = self.GetComponent<Enemy>();

            // �����̓r�w�C�r�A�c���[�̃m�[�h���s���Ƃ����݌v��̗��R��
            // �ŏ��ɃA�j���[�V�������Đ����邽�߂�����Sequence�ɂ��Ă���B
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
            // ��莞�Ԍ��Search�ɑJ��
            // Idle�̃A�j���[�V�����Đ��͂��̃V�[�P���X���Ȃ̂ŁA��������ɏ�������B
            if (_waitForTimeSequence.Update() == Node.State.Success)
            {
                if (_self.TryGetState(StateKey.Search, out State next))
                {
                    if (TryChangeState(next)) return;
                }
            }

            // �v���C���[���������ꍇ��Chase�ɑJ��
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