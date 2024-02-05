using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public static class CharacterExtensions
    {
        /// <summary>
        /// 現在位置から移動量分だけ指定した時間で移動する。
        /// </summary>
        public static async UniTask MoveAsync(this Transform t, Vector3 movement, float duration, CancellationToken skip)
        {
            Vector3 from = t.position;
            Vector3 to = from + movement;

            // 0除算防止
            if (duration > 0)
            {
                for (float f = 0; f < 1; f += Time.deltaTime / duration)
                {
                    // スキップした場合はその場に止まる
                    if (skip.IsCancellationRequested) break;

                    t.position = Vector3.Lerp(from, to, f);
                    await UniTask.Yield();
                }
            }

            t.position = to;
        }

        /// <summary>
        /// 現在の向きから指定した回転量を指定した時間をかけて回転する。
        /// </summary>
        public static async UniTask RotateAsync(this Transform t, float euler, float duration, CancellationToken skip)
        {
            Vector3 from = t.eulerAngles;
            Vector3 to = from + new Vector3(0, euler, 0);

            // 0除算防止
            if (duration > 0)
            {
                for (float f = 0; f < 1; f += Time.deltaTime / duration)
                {
                    // スキップした場合はその場に止まる
                    if (skip.IsCancellationRequested) break;

                    t.eulerAngles = Vector3.Lerp(from, to, f);
                    await UniTask.Yield();
                }
            }

            t.eulerAngles = to;
        }
    }
}
