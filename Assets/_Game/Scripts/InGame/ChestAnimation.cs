using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    public class ChestAnimation : BillboardAnimation
    {
        [SerializeField] GameObject _effect;
        [SerializeField] Collider _collider;

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
    }
}
