using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    public class DoorAnimation : BillboardAnimation
    {
        [SerializeField] Transform _left;
        [SerializeField] Transform _right;
        [SerializeField] Collider _collider;
        [Header("アニメーションの設定")]
        [SerializeField] float _duration;
        [SerializeField] float _openAngle;

        float _leftDefaultAngle;
        float _rightDefaultAngle;
        float _leftTargetAngle;
        float _rightTargetAngle;

        protected override void StartOverride()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // トリガーに出入りしたタイミングを検知
            _collider.isTrigger = true;

            _leftDefaultAngle = _left.localEulerAngles.y;
            _rightDefaultAngle = _right.localEulerAngles.y;
            _leftTargetAngle = _openAngle;
            _rightTargetAngle = _openAngle;

            // 範囲内に入ったら開いた角度、出たら閉じた角度
            _collider.OnTriggerEnterAsObservable().Where(c => c.TryGetComponent(out Player _)).Subscribe(_ => 
            {
                _leftTargetAngle = _openAngle;
                _rightTargetAngle = _openAngle;
            });
            _collider.OnTriggerExitAsObservable().Where(c => c.TryGetComponent(out Player _)).Subscribe(_ => 
            {
                _leftTargetAngle = _leftDefaultAngle;
                _rightTargetAngle = _rightDefaultAngle;
            });
        }
        
        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // 毎フレーム目標の角度に向けて回転する
                _left.localEulerAngles = new Vector3(0, Mathf.Lerp(_left.localEulerAngles.y, _leftTargetAngle, Time.deltaTime / _duration), 0);
                _right.localEulerAngles = new Vector3(0, Mathf.Lerp(_right.localEulerAngles.y, _rightTargetAngle, Time.deltaTime / _duration), 0);
                
                await UniTask.Yield(token);
            }
        }
    }
}
