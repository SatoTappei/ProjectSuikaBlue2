using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    public class BillboardAnimation : MonoBehaviour
    {
        [SerializeField] Transform _fbx;

        void Start()
        {
            StartOverride();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        /// <summary>
        /// Start�̃^�C�~���O�ŌĂ΂��
        /// </summary>
        protected virtual void StartOverride() { }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // �݌v�I�ɂ�DI���ׂ������A�ړI���P���Ȃ̂Ń^�O�Ŏ擾
            GameObject player = GameObject.FindGameObjectWithTag(Const.PlayerTag);

            if (player == null)
            {
                Debug.LogWarning("Player�^�O�������̂Ńr���{�[�h��������");
                return;
            }

            Transform t = player.transform;
            while (!token.IsCancellationRequested)
            {
                // �r���{�[�h
                Vector3 p = t.position;
                p.y = transform.position.y;
                transform.LookAt(p);

                await UniTask.Yield(token);
            }
        }
    }
}
