using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Novel
{
    public static class MoveExtensions
    {
        public static async UniTask MoveRelativeAsync(this Transform transform, Vector2 to, 
            float duration, Vector3 skip, CancellationToken token)
        {
            for (float t = 0; t < 1; t += Time.deltaTime / duration)
            {
                // スキップされた際に元の位置に戻る場合と、移動先に位置を変更する場合がある
                if (token.IsCancellationRequested) { transform.position = skip; return; }

                transform.position += (Vector3)Vector2.Lerp(Vector2.zero, to, t);
                await UniTask.Yield();
            }

            transform.position += (Vector3)to;
        }
    }
}
