using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;

namespace PSB.Game
{
    public class InGameEntryPoint : MonoBehaviour
    {
        GameState _gameState;
        DungeonManager _dungeonManager;
        UiManager _uiManager;

        [Inject]
        void Construct(GameState gameState, DungeonManager dungeonManager, UiManager uiManager)
        {
            _gameState = gameState;
            _dungeonManager = dungeonManager;
            _uiManager = uiManager;
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // ダンジョン生成中にロード画面
            using (CancellationTokenSource cts = new())
            {
                _uiManager.LoadingAsync(cts.Token).Forget();
                await _dungeonManager.BuildAsync(token);
                cts.Cancel();
            }

            // ゲーム開始の準備が完了
            _gameState.IsInGameReady = true;

            // 宝箱を取るまで待つ
            await UniTask.WaitUntil(() => _gameState.IsGetTreasure);
            // 入口に立つまで待つ
            await UniTask.WaitUntil(() => _gameState.IsStandingEntrance);

            // ゲーム終了条件達成
            _gameState.IsInGameClear = true;

            // ゲームクリアの演出
            await _uiManager.GameClearAnimationAsync(token);
        }

        // ギズモに描画するイベント関数を使用するためにVContainerのエントリポイントを使用せずMonoBehaviorを使う。
        // 専用のクラスを作れば良いがそこまでする必要性。
        void OnDrawGizmos()
        {
            if(_dungeonManager != null) _dungeonManager.DrawOnGizmos();
        }
    }
}
