using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// �o�H�ɉ����Ĉړ�����A�N�V�����m�[�h�B
    /// ���ɏ������܂ꂽ�o�H�ɉ����Ĉړ�����̂ŁA��ɍ��Ɍo�H���������ޕK�v������B
    /// �A�j���[�V�����̍Đ��@�\�͖����B
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

            // �o�H�������ꍇ�͎��s
            if (path == null) return State.Failure;            

            if (_progress >= 1.0f)
            {
                if (_next == path.Count) return State.Success;

                _progress = 0;
                _moveFrom = _self.GetPosition().position;
                _moveTo = path[_next].Position;
                _indexTo = path[_next].Index;
                _next++;
                // �x�N�g�����m�ŕ��������߂�̂Ɠ�����@�ŁA�㉺���E��\���P�ʃx�N�g�������߂�B
                _rotateTo = _indexTo - _self.GetPosition().index;

                // ���̃Z�������Ƀv���C���[������ꍇ�͂���ȏ�i�߂Ȃ��B
                if (_self.IsExistPlayer(_indexTo)) return State.Success;

                // �v���C���[�����m�����ꍇ�͑ł��؂�B
                if (_self.DetectPlayer(_detectDistance, checkWithinSight: true)) return State.Success;
            }
            else
            {
                // ���ɖڕW�̈ʒu�ɓ��B���Ă���ꍇ
                if (_moveTo == _self.GetPosition().position) _progress = 1.0f;

                _progress += Time.deltaTime * _moveSpeed;
                _progress = Mathf.Clamp01(_progress);

                Vector3 p = Vector3.Lerp(_moveFrom, _moveTo, _progress);
                _self.SetPosition(p, _indexTo);

                // �㉺���E�Ɉړ�����ꍇ�͉�]����
                if (_rotateTo != Vector2Int.zero)
                {
                    _self.Rotate(_rotateTo.ToDirection(), Time.deltaTime * _rotSpeed);
                }
            }

            return State.Running;
        }

        // Enter��Exit�̃^�C�~���O�Ń��Z�b�g
        void ResetParameters()
        {
            _next = 0;

            // ���_�Ԃ̈ړ�������������ԂɂȂ�̂ŁA�ŏ���Stay�̌Ăяo����0�Ԗڂ̒��_�ֈړ��悪�X�V�����B
            _progress = 1;

            // �ړ������ړ�������݂̍��W�ƈʒu�Ƀ��Z�b�g�B
            (Vector3 p, Vector2Int i) = _self.GetPosition();
            _moveFrom = p;
            _moveTo = p;
            _indexTo = i;

            _rotateTo = Vector2Int.zero;
        }
    }
}

