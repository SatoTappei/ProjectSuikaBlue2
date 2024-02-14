using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.Game.BT
{
    /// <summary>
    /// �r�w�C�r�A�c���[�̊e��m�[�h�͂��̃N���X���p������B
    /// </summary>
    public abstract class Node
    {
        public enum State
        {
            Running,
            Failure,
            Success,
        }

        State _state;
        bool _isActive;

        public Node(string name = null)
        {
            Name = name ?? "BehaviorTreeNode";
        }

        /// <summary>
        /// ���O��UI���ɕ\�����邽�߂̃m�[�h��
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 1��ȏ�Update���Ă񂾏�Ԃ���m�[�h�������I�ɏ�����Ԃɖ߂��ۂɌĂԁB
        /// </summary>
        public void Break()
        {
            _state = State.Running;
            _isActive = false;

            OnBreak();
        }

        /// <summary>
        /// 1�x�̌Ăяo���ōŏ���1���Enter��Stay���Ă΂��B
        /// Stay��Running�ȊO��Ԃ����ꍇ��Stay��Exit���Ă΂��
        /// </summary>
        public State Update()
        {
            if (!_isActive)
            {
                _isActive = true;
                Enter();
            }

#if UNITY_EDITOR
            //Debug.Log(NodeName + "�����s��");
#endif

            _state = Stay();

            if (_state == State.Failure || _state == State.Success)
            {
                Exit();
                _isActive = false;
            }

            return _state;
        }

        protected virtual void OnBreak() { }
        protected abstract void Enter();
        protected abstract State Stay();
        protected abstract void Exit();
    }
}
