using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using VContainer;

namespace PSB.Game
{
    public class マルチスレッド処理 : MonoBehaviour
    {
        DungeonManager _dungeonManager;
        System.Diagnostics.Stopwatch _stopwatch = new();

        [Inject]
        void Construct(DungeonManager dungeonManager)
        {
            _dungeonManager = dungeonManager;
        }

        void Start()
        {
#if false
            // 並列処理でfor文を回す。第1と第2引数が配列の添え字、第3引数が添え字を引数としたアクション。
            Parallel.For(0, 10, index => 
            {
                Debug.Log($"{index}番目: {Thread.CurrentThread.ManagedThreadId}");
            });
            Debug.Log("並列ループ完了");
#endif
#if false
            // 普通にfor文を回す。
            System.Diagnostics.Stopwatch sw = new();
            sw.Start();
            int c = 0;
            for (int i = 0; i < 1000000; i++)
            {
                c++;
            }
            sw.Stop();
            Debug.Log($" シングルスレッド処理時間: {sw.Elapsed} ms"); // .0017099ms

            // 並列処理でfor文を回す。
            System.Diagnostics.Stopwatch sw2 = new();
            sw2.Start();
            ParallelOptions option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 2;
            Parallel.For(0, 1000000, option, index =>
            {
                c++;
            });
            sw2.Stop();
            Debug.Log($" 並列処理時間: {sw2.Elapsed} ms"); // .0091658ms
#endif
#if false
            {
                System.Diagnostics.Stopwatch sw = new();
                sw.Start();
                for (int i = 0; i < 100; i++) 重い処理();
                sw.Stop();
                Debug.Log($"順次処理時間: {sw.Elapsed} ms"); // .0211819
            }
            {
                Action[] a = new Action[100];
                for(int i = 0; i < a.Length; i++)
                {
                    a[i] = 重い処理;
                }
                System.Diagnostics.Stopwatch sw = new();
                sw.Start();
                Parallel.Invoke(a);
                sw.Stop();
                Debug.Log($"並列処理時間: {sw.Elapsed} ms"); // .0023154
            }
#endif
            {
                System.Diagnostics.Stopwatch sw = new();
                sw.Start();
                IEnumerable<int> enu = Enumerable.Range(1000000, 500);
                var even = enu.Where(i => i % 2 == 0).Select(i => i.ToString());
                sw.Stop();
                Debug.Log($"処理時間: {sw.Elapsed} ms"); // .0000120ms
            }
            {
                System.Diagnostics.Stopwatch sw = new();
                sw.Start();
                IEnumerable<int> enu = Enumerable.Range(1000000, 500);
                var even = enu.AsParallel().Where(i => i % 2 == 0).Select(i => i.ToString());
                sw.Stop();
                Debug.Log($"並列処理時間: {sw.Elapsed} ms"); // .0000035ms
            }

            //UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void 重い処理()
        {
            for(int i = 0; i < 1000000; i++) { }
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            await _dungeonManager.BuildAsync(token);

            await UniTask.SwitchToThreadPool();
            _stopwatch.Start();
            for (int i = 0; i < 1000; i++)
            {
                _dungeonManager.Pathfinding(new Vector2Int(4, 4), new Vector2Int(0, 8));
            }
            _stopwatch.Stop();
            Debug.Log($" 処理時間: {_stopwatch.Elapsed} ms");
            await UniTask.SwitchToMainThread();
        }

        // ギズモに描画するイベント関数を使用するためにVContainerのエントリポイントを使用せずMonoBehaviorを使う。
        // 専用のクラスを作れば良いがそこまでする必要性。
        void OnDrawGizmos()
        {
            if (_dungeonManager != null) _dungeonManager.DrawOnGizmos();
        }
    }
}
