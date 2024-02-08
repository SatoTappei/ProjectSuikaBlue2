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
        [Header("�A�j���[�V�����̐ݒ�")]
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
            // �g���K�[�ɏo���肵���^�C�~���O�����m
            _collider.isTrigger = true;

            _leftDefaultAngle = _left.localEulerAngles.y;
            _rightDefaultAngle = _right.localEulerAngles.y;
            _leftTargetAngle = _openAngle;
            _rightTargetAngle = _openAngle;

            // �͈͓��ɓ�������J�����p�x�A�o��������p�x
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
                // ���t���[���ڕW�̊p�x�Ɍ����ĉ�]����
                _left.localEulerAngles = new Vector3(0, Mathf.Lerp(_left.localEulerAngles.y, _leftTargetAngle, Time.deltaTime / _duration), 0);
                _right.localEulerAngles = new Vector3(0, Mathf.Lerp(_right.localEulerAngles.y, _rightTargetAngle, Time.deltaTime / _duration), 0);
                
                await UniTask.Yield(token);
            }
        }
    }
}
