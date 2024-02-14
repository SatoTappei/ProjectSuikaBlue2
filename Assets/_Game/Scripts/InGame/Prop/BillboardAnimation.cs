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
        /// Startのタイミングで呼ばれる
        /// </summary>
        protected virtual void StartOverride() { }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // 設計的にはDIすべきだが、目的が単純なのでタグで取得
            Transform player = GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;

            if (player == null)
            {
                Debug.LogWarning("Playerタグが無いのでビルボードが無効化");
                return;
            }

            while (!token.IsCancellationRequested)
            {
                // ビルボード
                Vector3 p = player.position;
                p.y = transform.position.y;
                transform.LookAt(p);

                await UniTask.Yield(token);
            }
        }
    }
}
