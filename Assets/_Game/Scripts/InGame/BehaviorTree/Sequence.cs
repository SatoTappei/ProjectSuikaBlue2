using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    public class Sequence : Node
    {
        readonly List<Node> _children;

        // �q�����s���̏�Ԃ�Ԃ����ꍇ�ɁA���̌Ăяo���̃^�C�~���O��
        // 1�Ԗڂ̎q������s�����̂�h�����߂ɁA���s���̎q�̓Y������ێ����Ă����B
        int _current;

        public Sequence(string name = nameof(Sequence), params Node[] node) : base(name)
        {
            _children = new(node.Length);
            _children.AddRange(node);
        }

        protected override void OnBreak()
        {
            _current = 0;
            _children.ForEach(c => c.Break());
        }

        protected override void Enter()
        {
            _current = 0;
        }

        protected override void Exit()
        {
            _current = 0;
        }

        protected override State Stay()
        {
            while (_current < _children.Count)
            {
                State result = _children[_current].Update();

                // �q�����������ꍇ�͎��̌Ăяo����҂����Ɏ��̎q�����s
                if (result == State.Success) { _current++; continue; }

                // �q�����s���̏ꍇ�͂���ȏ���s���Ȃ��B
                // �q�����s�����ꍇ�͂��̃m�[�h���̂����s��Ԃ��̂ŁA���̌Ăяo�����͍ŏ��̎q������s�ɂȂ�B
                return result;
            }

            // �S�Ă̎q�����������ꍇ�͐�����Ԃ��B
            return State.Success;
        }

        /// <summary>
        /// �q�m�[�h�Ƃ��Ēǉ�
        /// </summary>
        public void Add(Node node) => _children.Add(node);
    }
}
