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
        [Header("アニメーションの設定")]
        [SerializeField] float _openAngle;
        [SerializeField] float _duration;

        // 開くアニメーションは最初の1回のみ再生
        bool _isFirst = true;

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

        // 外部からの呼び出して蓋を開けるアニメーションする
        void IInteractive.Action(object arg)
        {
            if (!_isFirst) return;
            _isFirst = false;

            OpenAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // 蓋が開くアニメーション
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
