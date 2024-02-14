using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// �L�����N�^�[�����o����A�N�V�����m�[�h
    /// </summary>
    public class CharacterDetect : Node
    {
        readonly Enemy _self;
        readonly int _distance;

        public CharacterDetect(int distance, Enemy self, string name = nameof(CharacterDetect)) : base(name)
        {
            _distance = distance;
            _self = self;
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
            // �v���C���[�Ƃ̋��������ȉ����A�����Ă��邩
            if (_self.DetectPlayer(_distance, checkWithinSight: true))
            {
                return State.Success;
            }
            else return State.Failure;
        }
    }
}
