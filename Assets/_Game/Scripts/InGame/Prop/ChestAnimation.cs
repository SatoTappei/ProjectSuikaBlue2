using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class ChestAnimation : BillboardAnimation, ILocation
    {
        [SerializeField] GameObject _effect;
        [SerializeField] Collider _collider;
        [SerializeField] Transform _cover;
        [Header("�A�j���[�V�����̐ݒ�")]
        [SerializeField] float _openAngle;
        [SerializeField] float _duration;

        // �J���A�j���[�V�����͍ŏ���1��̂ݍĐ�
        bool _isFirst = true;

        protected override void StartOverride()
        {
            Init();
        }

        void Init()
        {
            _effect.SetActive(false);

            // �g���K�[�ɏo���肵���^�C�~���O�����m
            _collider.isTrigger = true;

            // �͈͓��ɓ������牉�o�̍Đ��A�o�����~
            _collider.OnTriggerEnterAsObservable().Where(c => c.TryGetComponent(out Player _)).Subscribe(_ =>
            {
                _effect.SetActive(true);
            });
            _collider.OnTriggerExitAsObservable().Where(c => c.TryGetComponent(out Player _)).Subscribe(_ =>
            {
                _effect.SetActive(false);
            });
        }

        // �O������̌Ăяo���ĊW���J����A�j���[�V��������
        void IInteractive.Action(object arg)
        {
            if (!_isFirst) return;
            _isFirst = false;

            OpenAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // �W���J���A�j���[�V����
        async UniTaskVoid OpenAsync(CancellationToken token)
        {
            float from = _cover.transform.localEulerAngles.z;
            for (float t = 0; t < 1; t += Time.deltaTime / _duration)
            {
                _cover.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(from, _openAngle, t));
                await UniTask.Yield(token);
            }
        }
    }
}
