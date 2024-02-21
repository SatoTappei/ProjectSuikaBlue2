using Cysharp.Threading.Tasks;
using PSB.Architect;
using PSB.Game;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace PSB.Game
{
    public class ダンジョン生成 : MonoBehaviour
    {
        DungeonManager _dungeonManager;

        [Inject]
        void Construct(DungeonManager dungeonManager)
        {
            _dungeonManager = dungeonManager;
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            await _dungeonManager.BuildAsync(token);
        }

        // ギズモに描画するイベント関数を使用するためにVContainerのエントリポイントを使用せずMonoBehaviorを使う。
        // 専用のクラスを作れば良いがそこまでする必要性。
        void OnDrawGizmos()
        {
            if (_dungeonManager != null) _dungeonManager.DrawOnGizmos();
        }
    }

}