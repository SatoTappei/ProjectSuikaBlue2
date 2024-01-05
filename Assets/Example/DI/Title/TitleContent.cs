using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using VContainer;

namespace PSB.DI.Title
{
    /// <summary>
    /// �^�C�g����ʂ̊e�R���e���c�B
    /// VContainer�ɂ���Ē������ꂽ�A�^�C�g���̏�Ԃ��Ǘ�����N���X�̒l���Ď����A
    /// ���g�̏�ԂɑJ�ڂ����ꍇ�ɊJ���A���̏�ԂɑJ�ڂ����ꍇ�͕���B
    /// </summary>
    public class TitleContent : MonoBehaviour
    {
        [Header("��ԂɕR�Â���")]
        [SerializeField] TitleState _contentState;
        [Header("���삷��UI")]
        [SerializeField] Button _openButton;
        [SerializeField] Button _backButton;
        [Tooltip("����{�^�����������^�C�~���O�ł��̃I�u�W�F�N�g����\���ɂȂ�")]
        [SerializeField] GameObject _root;

        StateOfTitle _state;

        [Inject]
        public void Construct(StateOfTitle state)
        {
            _state = state;
        }

        void Start()
        {
            // �J���{�^�����N���b�N�����ۂɁA���݂̏�Ԃ����̃R���e���c�̏�ԂɕύX����
            _openButton.OnClickAsObservable().Subscribe(_ => 
            {
                _state.ChangeState(_contentState);
            }).AddTo(gameObject);

            // ���݂̏�Ԃ����̃R���e���c�̏�Ԃ̏ꍇ�͊J���A����ȊO�̏ꍇ�͕���
            _state.CurrentState.Subscribe(state => 
            {
                _root.SetActive(state == _contentState);
            }).AddTo(gameObject);

            // �߂�{�^�����N���b�N�������ۂɁA���݂̏�Ԃ��g�b�v�ɕύX���邱�Ƃŕ���
            _backButton.OnClickAsObservable().Subscribe(_ => 
            {
                _state.ChangeState(TitleState.Top);
            }).AddTo(gameObject);
        }
    }
}
