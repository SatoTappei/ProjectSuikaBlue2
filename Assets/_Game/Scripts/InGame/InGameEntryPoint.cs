using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class InGameEntryPoint : MonoBehaviour
    {
        [SerializeField] DungeonManager _dungeonManager;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            await _dungeonManager.BuildAsync(token);

            // �v���C���[�̑�����󂯕t���鏀������
            MessageBroker.Default.Publish(new InGameReadyMessage());
        }
    }
}
