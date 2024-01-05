using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Token
{
    public bool Cancel;
}

public class Sample : MonoBehaviour
{
    [SerializeField] 
    int _count = 5;

    [SerializeField]
    private float _duration = 1F; // アニメーション時間（秒単位）

    [SerializeField]
    private Color _from = Color.clear; // この色からフェード処理する

    [SerializeField]
    private Color _to = Color.white; // この色に向かってフェード処理する

    IEnumerator Start()
    {
        Image[] images = new Image[_count];
        for (int i = 0; i < _count; i++)
        {
            // Image コンポーネントを持つオブジェクトを生成
            var obj = new GameObject("Image");
            obj.transform.parent = transform;
            var image = obj.AddComponent<Image>();
            image.color = _from;

            images[i] = image;
        }

        foreach (Image image in images)
        {
            // 投げっぱなし(Fire and Forget)
            // ★メンバに保持した変数でも引数として渡してやると、メソッド内では引数以外の値を減らすことが出来る。
            var c1 = StartCoroutine(FadeAsync(image, _duration));
            var c2 = StartCoroutine(ShakeAsync(image.transform, _duration));

            // StartCoroutine が返した Coroutine オブジェクトを待機できる
            // 両方の終了を待ちたいなら、それぞれ yield return すればよい
            yield return c1;
            yield return c2;
        }
    }

    IEnumerator FadeAsync(Image image, float duration)
    {
        // ★whileをforに変更したので簡略化されている。
        for (float p = 0; p < 1; p += Time.deltaTime / duration)
        {
            image.color = Color.Lerp(_from, _to, p);
            yield return null;
        }

        image.color = Color.Lerp(_from, _to, 1);
    }

    IEnumerator AnimationAsync(Transform transform, float duration)
    {
        Vector3 from = transform.eulerAngles;
        Vector3 left = new Vector3(0, 0, 45);
        Vector3 right = new Vector3(0, 0, -45);

        yield return RotationAsync(transform, from, left, duration);
        yield return RotationAsync(transform, left, right, duration);
        yield return RotationAsync(transform, right, from, duration);
    }

    IEnumerator RotationAsync(Transform transform, Vector3 a, Vector3 b, float duration)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            transform.eulerAngles = Vector3.Lerp(a, b, progress);
            yield return null;
        }
    }

    // 赤坂講師回答
    IEnumerator ShakeAsync(Transform target, float duration)
    {
        for (var t = 0F; t < duration; t += Time.deltaTime)
        {
            var p = t / duration;
            var r = 20 * Mathf.Sin(Mathf.PI * 2 * p);
            target.eulerAngles = new Vector3(0, 0, r);
            yield return null;
        }
    }

    // 赤坂講師回答(並列したコルーチンの実行)
    IEnumerator WhenAll(params IEnumerator[] enumerators)
    {
        // コルーチンを全て起動
        var awaiters = new Coroutine[enumerators.Length];
        for (var i = 0; i < awaiters.Length; i++)
        {
            var e = enumerators[i];
            awaiters[i] = StartCoroutine(e);
        }

        // 起動したコルーチンを全て待つ
        foreach (var awaiter in awaiters)
        {
            yield return awaiter;
        }
    }
}