using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// �אڂ���Z���ւ̌o�H�����߂�A�N�V�����m�[�h�B
    /// </summary>
    public class PathToNeighbour : Node
    {
        readonly Enemy _self;

        public PathToNeighbour(Enemy self, string name = nameof(PlayAnimation)) : base(name)
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
            Vector2Int current = _self.GetPosition().index;
            foreach (Vector2Int d in Utility.RandomDirection(Utility.RandomSeed()))
            {
                // �㉺���E�̂��������_���ȕ����ׂ̗̃Z����I���B
                // ���̕����̃Z���Ɉړ��ł��邩�̕ۏ؂͂��̃m�[�h�ł͍s��Ȃ��B
                Vector2Int neighbour = current + d;
                _self.PrivateBoard.Path = _self.Pathfinding(neighbour);
                
                break;
            }

            return State.Success;
        }
    }
}