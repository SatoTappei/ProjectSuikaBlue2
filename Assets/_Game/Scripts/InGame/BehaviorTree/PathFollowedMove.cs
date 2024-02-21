using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// 経路に沿って移動するアクションノード。
    /// 黒板に書き込まれた経路に沿って移動するので、先に黒板に経路を書き込む必要がある。
    /// アニメーションの再生機能は無い。
    /// </summary>
    public class PathFollowedMove : Node
    {
        readonly float _moveSpeed;
        readonly float _rotSpeed;
        readonly int _detectDistance;
        readonly Enemy _self;

        int _next;
        float _progress;
        Vector3 _moveFrom;
        Vector3 _moveTo;
        Vector2Int _indexTo;
        Vector2Int _rotateTo;

        public PathFollowedMove(float moveSpeed, float rotSpeed, int detectDistance, Enemy self, 
            string name = nameof(Sequence)) : base(name)
        {
            _moveSpeed = moveSpeed;
            _rotSpeed = rotSpeed;
            _detectDistance = detectDistance;
            _self = self;
        }

        protected override void OnBreak()
        {
            ResetParameters();
        }

        protected override void Enter()
        {
            ResetParameters();
        }

        protected override void Exit()
        {
            ResetParameters();
        }

        protected override State Stay()
        {
            IReadOnlyList<IReadOnlyCell> path = _self.PrivateBoard.Path;

            // 経路が無い場合は失敗
            if (path == null) return State.Failure;            

            if (_progress >= 1.0f)
            {
                if (_next == path.Count) return State.Success;

                _progress = 0;
                _moveFrom = _self.GetPosition().position;
                _moveTo = path[_next].Position;
                _indexTo = path[_next].Index;
                _next++;
                // ベクトル同士で方向を求めるのと同じ手法で、上下左右を表す単位ベクトルを求める。
                _rotateTo = _indexTo - _self.GetPosition().index;

                // 次のセルが既にプレイヤーがいる場合はこれ以上進めない。
                if (_self.IsExistPlayer(_indexTo)) return State.Success;

                // プレイヤーを検知した場合は打ち切る。
                if (_self.DetectPlayer(_detectDistance, checkWithinSight: true)) return State.Success;
            }
            else
            {
                // 既に目標の位置に到達している場合
                if (_moveTo == _self.GetPosition().position) _progress = 1.0f;

                _progress += Time.deltaTime * _moveSpeed;
                _progress = Mathf.Clamp01(_progress);

                Vector3 p = Vector3.Lerp(_moveFrom, _moveTo, _progress);
                _self.SetPosition(p, _indexTo);

                // 上下左右に移動する場合は回転する
                if (_rotateTo != Vector2Int.zero)
                {
                    _self.Rotate(_rotateTo.ToDirection(), Time.deltaTime * _rotSpeed);
                }
            }

            return State.Running;
        }

        // EnterとExitのタイミングでリセット
        void ResetParameters()
        {
            _next = 0;

            // 頂点間の移動が完了した状態になるので、最初のStayの呼び出しで0番目の頂点へ移動先が更新される。
            _progress = 1;

            // 移動元も移動先も現在の座標と位置にリセット。
            (Vector3 p, Vector2Int i) = _self.GetPosition();
            _moveFrom = p;
            _moveTo = p;
            _indexTo = i;

            _rotateTo = Vector2Int.zero;
        }
    }
}

