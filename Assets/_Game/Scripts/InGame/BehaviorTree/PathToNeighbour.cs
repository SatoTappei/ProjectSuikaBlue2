using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// 隣接するセルへの経路を求めるアクションノード。
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
                // 上下左右のうちランダムな方向の隣のセルを選択。
                // その方向のセルに移動できるかの保証はこのノードでは行わない。
                Vector2Int neighbour = current + d;
                _self.PrivateBoard.Path = _self.Pathfinding(neighbour);
                
                break;
            }

            return State.Success;
        }
    }
}