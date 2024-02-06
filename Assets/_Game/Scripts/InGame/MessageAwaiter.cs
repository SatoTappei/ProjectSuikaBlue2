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
        /// メッセージを受信するまで待機する
        /// </summary>
        public static async UniTask<T> ReceiveAsync<T>(CancellationToken token)
        {
            return await MessageBroker.Default.Receive<T>().ToUniTask(useFirstValue: true, token);
        }
    }
}
