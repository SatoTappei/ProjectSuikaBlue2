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
        // 自身にメッセージングする構造体
        struct Message { }

        [System.Serializable]
        class ShakeParameter
        {
            public float Duration;
            public float Strength;
            public int Vibrato;
        }

        [Header("カメラ側の各オブジェクト")]
        [SerializeField] Transform _root;
        [SerializeField] Transform _parent;
        [SerializeField] Transform _camera;
        [Header("追従する対象")]
        [SerializeField] Transform _follow;
        [Header("位置のオフセット")]
        [SerializeField] Vector3 _offset;
        [Header("振動する際のパラメータ")]
        [SerializeField] ShakeParameter _shakeParameter;

        Vector3 _shakeAngle;

        void Awake()
        {
            // 自身にメッセージングすることでカメラを揺らす。
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
                // rootオブジェクトは追従対象と同じ位置
                _root.position = _follow.position;
                // parentオブジェクトで位置のオフセット
                _parent.localPosition = _offset;
                // camera本体が回転して対象と同じ方向を向く
                _camera.forward = _follow.forward;
                _camera.localEulerAngles += _shakeAngle;

                await UniTask.Yield(PlayerLoopTiming.LastUpdate, cancellationToken: token);
            }
        }

        /// <summary>
        /// カメラを揺らす
        /// </summary>
        public static void Shake()
        {
            MessageBroker.Default.Publish(new Message());
        }

        // カメラが揺れる演出
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
