using System.Collections;
using System.Threading;
using UnityEngine;

public class CoroutineProvider : MonoBehaviour
{
    /// <summary>
    /// 外から、何か継続的な処理の開始を指示する。
    public Coroutine Run(string name, CancellationToken token)
    {
        return StartCoroutine(RunCoroutine(name, token));
    }

    private IEnumerator RunCoroutine(string name, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 何か継続処理…
            //Debug.Log($"{name}: {Time.frameCount}");
            yield return null;
        }

        //token.Register(name => Debug.Log("キャンセルした: " + name));

        Debug.Log($"{name}が終了しました");
    }
}