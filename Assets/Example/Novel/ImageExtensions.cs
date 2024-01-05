using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Novel
{
    /// <summary>
    /// 拡張メソッドの条件はstaticクラスかつ、staticメソッドかつ第一引数がthisキーワードがついていること。
    /// </summary>
    public static class ImageExtensions
    {
        public static async UniTask FadeInAsync(this Image image, float duration, CancellationToken token)
        {
            Color from = image.color;
            from.a = 0;

            Color to = image.color;
            to.a = 1;
            await FadeAsync(image, from, to, duration, token);
        }

        public static async UniTask FadeOutAsync(this Image image, float duration, CancellationToken token)
        {
            Color from = image.color;
            from.a = 1;

            Color to = image.color;
            to.a = 0;
            await FadeAsync(image, from, to, duration, token);
        }

        public static async UniTask FadeAsync(this Image image, Color from, Color to, float duration, CancellationToken token)
        {
            for (var t = 0F; t < duration; t += Time.deltaTime)
            {
                if (token.IsCancellationRequested) break;

                image.color = Color.Lerp(from, to, t / duration);
                await UniTask.Yield();
            }

            image.color = to;
        }
    }
}