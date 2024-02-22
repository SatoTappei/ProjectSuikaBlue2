using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UniRx;

namespace PSB.Game
{
    public class CameraController : MonoBehaviour
    {
        // ���g�Ƀ��b�Z�[�W���O����\����
        struct Message { }

        [System.Serializable]
        class ShakeParameter
        {
            public float Duration;
            public float Strength;
            public int Vibrato;
        }

        [Header("�J�������̊e�I�u�W�F�N�g")]
        [SerializeField] Transform _root;
        [SerializeField] Transform _parent;
        [SerializeField] Transform _camera;
        [Header("�Ǐ]����Ώ�")]
        [SerializeField] Transform _follow;
        [Header("�ʒu�̃I�t�Z�b�g")]
        [SerializeField] Vector3 _offset;
        [Header("�U������ۂ̃p�����[�^")]
        [SerializeField] ShakeParameter _shakeParameter;

        Vector3 _shakeAngle;

        void Awake()
        {
            // ���g�Ƀ��b�Z�[�W���O���邱�ƂŃJ������h�炷�B
            MessageBroker.Default.Receive<Message>().Subscribe(_ => 
            {
                Shake(_shakeParameter.Duration, _shakeParameter.Strength, _shakeParameter.Vibrato);
            }).AddTo(this);
        }

        void Start()
        {
            if (_follow == null) return;

            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // root�I�u�W�F�N�g�͒Ǐ]�ΏۂƓ����ʒu
                _root.position = _follow.position;
                // parent�I�u�W�F�N�g�ňʒu�̃I�t�Z�b�g
                _parent.localPosition = _offset;
                // camera�{�̂���]���đΏۂƓ�������������
                _camera.forward = _follow.forward;
                _camera.localEulerAngles += _shakeAngle;

                await UniTask.Yield(PlayerLoopTiming.LastUpdate, cancellationToken: token);
            }
        }

        /// <summary>
        /// �J������h�炷
        /// </summary>
        public static void Shake()
        {
            MessageBroker.Default.Publish(new Message());
        }

        // �J�������h��鉉�o
        void Shake(float duration, float strength, int vibrato)
        {
            DOTween.Shake(() => _shakeAngle, 
                angle => _shakeAngle = angle, 
                duration, 
                strength, 
                vibrato).OnComplete(() => _shakeAngle = Vector3.zero);
        }
    }
}
