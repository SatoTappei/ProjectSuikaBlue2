using PSB.Game.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    [System.Serializable]
    public class ChaseState : State
    {
        [Header("�ړ����x")]
        [SerializeField] float _moveSpeed = 1;
        [Header("��]���x")]
        [SerializeField] float _rotSpeed = 10;
        [Header("����ۂ̃p�[�e�B�N���Đ��Ԋu")]
        [SerializeField] float _particleInterval = 0.25f;

        Enemy _self;
        Sequence _sequence;

        public override StateKey Key => StateKey.Chase;

        protected override void Init(GameObject self)
        {
            _self = self.GetComponent<Enemy>();

            // �o�H�T����A�o�H�ɉ����Ĉړ��B
            _sequence = new(nameof(Sequence),
                new Pathfinding(_self),
                new PlayAnimation(_self, Enemy.AnimationKey.Sprint, 0),
                new PlayParticle(_self, Enemy.ParticleKey.Dash, _particleInterval,
                    new PathFollowedMove(_moveSpeed, _rotSpeed, 0, _self)));
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
            // �ړ��������Attack�ɑJ��
            if (_sequence.Update() == Node.State.Success)
            {
                if (_self.TryGetState(StateKey.Attack, out State next))
                {
                    if (TryChangeState(next)) return;
                }
            }
        }
    }
}