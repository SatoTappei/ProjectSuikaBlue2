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

            // トリガーに出入りしたタイミングを検知
            _collider.isTrigger = true;

            // 範囲内に入ったら演出の再生、出たら停止
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
