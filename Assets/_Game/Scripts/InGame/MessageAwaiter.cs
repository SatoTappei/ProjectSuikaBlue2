using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace PSB.Game
{
    public static class MessageAwaiter
    {
        /// <summary>
        /// ���b�Z�[�W����M����܂őҋ@����
        /// </summary>
        public static async UniTask<T> ReceiveAsync<T>(CancellationToken token)
        {
            return await MessageBroker.Default.Receive<T>().ToUniTask(useFirstValue: true, token);
        }
    }
}
