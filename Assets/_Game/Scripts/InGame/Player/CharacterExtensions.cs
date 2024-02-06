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

        /// <summary>
        /// ジャンプ(設置判定が作りかけなので正常に動作しない)
        /// </summary>
        public static async UniTask JumpAsync(this Rigidbody rb, Vector2Int input, CancellationToken token,
            float horizontalPower, float verticalPower, float interval)
        {
            Vector3 force = Vector3.up * verticalPower + Vector3.right * input.x * horizontalPower;
            rb.AddForce(force, ForceMode.Impulse);

            // ジャンプ中に横方向に力を加え続けることで、段差に引っかかっても乗り越えることが出来る。
            for (float f = 0; f < interval; f += Time.fixedDeltaTime)
            {
                Vector3 velo = rb.velocity;
                velo.x = input.x * horizontalPower;
                rb.velocity = velo;
                // ジャンプした次のフレームではレイキャストが地面から離れないので、判定までのクールタイムを設ける。
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: token);
            }

            //await UniTask.WaitUntil(IsGrounding, cancellationToken: token);
        }
    }
}

// 優先:スキップのトークンだけ渡しているので回転中にエディター止めるとアクセスできないエラーが出る。