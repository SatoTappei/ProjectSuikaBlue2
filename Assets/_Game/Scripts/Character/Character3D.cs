using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

namespace PSB.Game
{
    public class Character3D : MonoBehaviour
    {
        /// <summary>
        /// �A�j���[�V�����Đ��p�̃L�[
        /// </summary>
        public enum State { Idle, Angry, Fun, Order, }

        [SerializeField] Animator _animator;
        [Header("�A�j���[�V������")]
        [SerializeField] string _idleAnimName = "Idle";
        [SerializeField] string _angryAnimName = "Angry";
        [SerializeField] string _funAnimName = "Fun";
        [SerializeField] string _orderAnimName = "Order";
        int _idleAnimationHash;
        int _angryAnimationHash;
        int _funAnimationHash;
        int _orderAnimationHash;

        Talk _talk;

        [Inject]
        void Construct(Talk talk)
        {
            _talk = talk;
        }

        void Start()
        {
            // �A�j���[�V�����̃n�b�V��
            _idleAnimationHash = Animator.StringToHash(_idleAnimName);
            _angryAnimationHash = Animator.StringToHash(_angryAnimName);
            _funAnimationHash = Animator.StringToHash(_funAnimName);
            _orderAnimationHash = Animator.StringToHash(_orderAnimName);

            // ���莞�̐S��̕ω��ʂŃA�j���[�V�����Đ��B
            // �S��̕ω��ʂɂ��Ă͕�������̃��[�����L�q�����e�L�X�g�t�@�C���ɏ����Ă���B
            _talk.DeltaMental.Skip(1).Subscribe(value =>
            {
                if (value < 1) Play(State.Angry);
                if (1 < value) Play(State.Fun);
                else Play(State.Order);
            }).AddTo(this);
        }

        /// <summary>
        /// �w�肵���A�j���[�V�������Đ�
        /// </summary>
        public void Play(State state)
        {
            if (state == State.Idle) _animator.Play(_idleAnimationHash);
            else if (state == State.Angry) _animator.Play(_angryAnimationHash);
            else if (state == State.Fun) _animator.Play(_funAnimationHash);
            else if (state == State.Order) _animator.Play(_orderAnimationHash);
        }
    }
}
