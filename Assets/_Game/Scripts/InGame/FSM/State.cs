using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    /// <summary>
    /// �X�e�[�g�}�V���̊e��X�e�[�g�͂��̃N���X���p������B
    /// �e�X�e�[�g�Ɏ��g�̃I�u�W�F�N�g��n�����ƂŁA�X�e�[�g���ɏ������Ăяo���Ă��炤�B
    /// </summary>
    [System.Serializable]
    public abstract class State
    {
        public enum StateKey
        {
            Base,
            Idle,
            Chase,
            Attack,
            Search,
        }

        enum Stage
        {
            Enter,
            Stay,
            Exit,
        }

        Stage _stage;
        State _next;
        // Update���ĂԑO��Awake���Ă�ŏ������ς݂��̃t���O
        bool _initCompleted;

        /// <summary>
        /// �O������ǂ̃X�e�[�g�Ȃ̂��𔻒肷�邽�߂Ɏg�p�B
        /// </summary>
        public abstract StateKey Key { get; }

        /// <summary>
        /// Update���ĂԑO�ɍŏ���1�񂾂��Ăяo���B
        /// </summary>
        public void Awake(GameObject self)
        {
            Init(self);
            _initCompleted = true;
        }

        /// <summary>
        /// 1�x�̌Ăяo���ŃX�e�[�g�̒i�K�ɉ�����Enter,Stay,Exit�̂����ǂꂩ1�����s�����B
        /// ���̌Ăяo���Ŏ��s�����X�e�[�g��Ԃ��B
        /// </summary>
        public State Update()
        {
            if (!_initCompleted)
            {
                Debug.LogWarning("Awake���Ă�ł��Ȃ����߁A���������Ă��Ȃ���Ԃł̎��s��: " + Key);
            }

            if (_stage == Stage.Enter)
            {
                Enter();
                _stage = Stage.Stay;
            }
            else if (_stage == Stage.Stay)
            {
                Stay();
            }
            else if (_stage == Stage.Exit)
            {
                Exit();
                _stage = Stage.Enter;

                return _next;
            }

            return this;
        }

        protected abstract void Init(GameObject self);
        protected abstract void Enter();
        protected abstract void Stay();
        protected abstract void Exit();

        /// <summary>
        /// ���Ƀv�[��������o��������Enter�ȊO����n�܂�̂�h���B
        /// �v�[���ɖ߂��ۂɌĂԕK�v������B
        /// </summary>
        protected void Reset() => _stage = Stage.Enter;

        /// <summary>
        /// Enter()���Ă΂�Ă��A�X�e�[�g�̑J�ڏ������Ă�ł��Ȃ��ꍇ�̂ݑJ�ډ\�B
        /// </summary>
        public bool TryChangeState(State next)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enter���Ă΂��O�ɃX�e�[�g��J�ڂ��邱�Ƃ͕s�\: {Key} �J�ڐ�: {next}");
                return false;
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"���ɕʂ̃X�e�[�g�ɑJ�ڂ��鏈�����Ă΂�Ă���: {Key} �J�ڐ�: {next}");
                return false;
            }

            _stage = Stage.Exit;
            _next = next;

            return true;
        }
    }
}
