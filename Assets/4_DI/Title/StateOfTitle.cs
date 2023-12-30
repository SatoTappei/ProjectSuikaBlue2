using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace PSB.DI.Title
{
    public enum TitleState
    {
        Top,
        StageSelect,
        Gacha,
        Option,
    }

    /// <summary>
    /// �^�C�g���̏�Ԃ�m�肽���N���X�͂��̃N���X���Q�Ƃ���B
    /// ���̂т��p�����Ă��Ȃ��̂ŃC���X�y�N�^���犄�蓖�Ă��Ȃ��B
    /// ������VContainer�ł��̃N���X�̃C���X�^���X�����A�K�v�ȃN���X�ɒ������Ă��B
    /// 1�̃N���X���Q�Ƃ��A�C���X�y�N�^�[�������Ȃ����AContainer�Ƃ����P�ʂŊǗ����ꂽ���̂��o���オ��
    /// </summary>
    public class StateOfTitle
    {
        ReactiveProperty<TitleState> _currentState = new(TitleState.Top);

        public IReadOnlyReactiveProperty<TitleState> CurrentState => _currentState;

        /// <summary>
        /// ��Ԃ̑J�ڂ��s���A�J�ڂ�Subscribe����Ă��鏈�������΂���B
        /// </summary>
        public void ChangeState(TitleState next)
        {
            _currentState.Value = next;
        }
    }
}
