using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// �v���C���[�܂ł̌o�H���擾����A�N�V�����m�[�h�B
    /// </summary>
    public class Pathfinding : Node
    {
        readonly Enemy _self;

        public Pathfinding(Enemy self, string name = nameof(Sequence)) : base(name)
        {
            _self = self;
        }

        protected override void OnBreak()
        {
        }

        protected override void Enter()
        {
            _self.PrivateBoard.Path = null;
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // �o�H�����ɏ������ށB
            _self.PrivateBoard.Path = _self.PathfindingToPlayer();

            return State.Success;
        }
    }
}
