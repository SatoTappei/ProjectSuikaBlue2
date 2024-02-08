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
        /// Œ»İˆÊ’u‚©‚çˆÚ“®—Ê•ª‚¾‚¯w’è‚µ‚½ŠÔ‚ÅˆÚ“®‚·‚éB
        /// </summary>
        public static async UniTask MoveAsync(this Transform t, Vector3 movement, float duration, CancellationToken token)
        {
            Vector3 from = t.position;
            Vector3 to = from + movement;

            // 0œZ–h~
            if (duration > 0)
            {
                for (float f = 0; f < 1; f += Time.deltaTime / duration)
                {
                    // ƒLƒƒƒ“ƒZƒ‹‚µ‚½ê‡‚Í‚»‚Ìê‚É~‚Ü‚é
                    if (token.IsCancellationRequested) break;

                    t.position = Vector3.Lerp(from, to, f);
                    await UniTask.Yield();
                }
            }

            t.position = to;
        }

        /// <summary>
        /// Œ»İ‚ÌŒü‚«‚©‚çw’è‚µ‚½‰ñ“]—Ê‚ğw’è‚µ‚½ŠÔ‚ğ‚©‚¯‚Ä‰ñ“]‚·‚éB
        /// </summary>
        public static async UniTask RotateAsync(this Transform t, float euler, float duration, CancellationToken token)
        {
            Vector3 from = t.eulerAngles;
            Vector3 to = from + new Vector3(0, euler, 0);

            // 0œZ–h~
            if (duration > 0)
            {
                for (float f = 0; f < 1; f += Time.deltaTime / duration)
                {
                    // ƒLƒƒƒ“ƒZƒ‹‚µ‚½ê‡‚Í‚»‚Ìê‚É~‚Ü‚é
                    if (token.IsCancellationRequested) break;

                    t.eulerAngles = Vector3.Lerp(from, to, f);
                    await UniTask.Yield();
                }
            }

            t.eulerAngles = to;
        }
    }
}