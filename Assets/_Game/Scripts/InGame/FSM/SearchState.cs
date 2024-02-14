using PSB.Game.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    [System.Serializable]
    public class SearchState : State
    {
        [Header("移動速度")]
        [SerializeField] float _moveSpeed = 0.5f;
        [Header("回転速度")]
        [SerializeField] float _rotSpeed = 10;

        Enemy _self;
        Sequence _sequence;

        public override StateKey Key => StateKey.Search;

        protected override void Init(GameObject self)
        {
            _self = self.GetComponent<Enemy>();

            // 隣のセルに移動。
            _sequence = new(nameof(Sequence),
                new PathToNeighbour(_self),
                new PlayAnimation(_self, Enemy.AnimationKey.Walk, 0),
                new PathFollowedMove(_moveSpeed, _rotSpeed, _self));
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
            // 移動が出来たかどうかに関わらずIdleに遷移
            if (_sequence.Update() == Node.State.Success)
            {
                if (_self.TryGetState(StateKey.Idle, out State next))
                {
                    if (TryChangeState(next)) return;
                }
            }
        }
    }
}
