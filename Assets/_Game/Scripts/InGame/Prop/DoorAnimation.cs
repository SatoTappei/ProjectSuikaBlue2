using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    public class DoorAnimation : BillboardAnimation, IInteractive
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
            _collider.OnTriggerEnterAsObservable()
                .Where(c => c.TryGetComponent(out Player _)).Subscribe(_ => Open());
            _collider.OnTriggerExitAsObservable()
                .Where(c => c.TryGetComponent(out Player _)).Subscribe(_ => Close());
        }

        // ���t���[���ڕW�̊p�x�Ɍ����ĉ�]����
        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                float l = Mathf.Lerp(_left.localEulerAngles.y, _leftTargetAngle, Time.deltaTime / _duration);
                _left.localEulerAngles = new Vector3(0, l, 0);
                float r = Mathf.Lerp(_right.localEulerAngles.y, _rightTargetAngle, Time.deltaTime / _duration);
                _right.localEulerAngles = new Vector3(0, r, 0);
                
                await UniTask.Yield(token);
            }
        }

        // �ڕW�̊p�x���J�����p�x�ɕύX
        void Open()
        {
            _leftTargetAngle = _openAngle;
            _rightTargetAngle = _openAngle;

            AudioPlayer.Play(AudioKey.DoorOpenSE, AudioPlayer.PlayMode.SE);
        }

        // �ڕW�̊p�x������p�x�ɕύX
        void Close()
        {
            _leftTargetAngle = _leftDefaultAngle;
            _rightTargetAngle = _rightDefaultAngle;

            AudioPlayer.Play(AudioKey.DoorCloseSE, AudioPlayer.PlayMode.SE);
        }

        void IInteractive.Action(object arg) { }
    }
}
