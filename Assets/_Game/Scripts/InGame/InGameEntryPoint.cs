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
            await _dungeonManager.BuildAsync(token);
            // プレイヤーの操作を受け付ける準備完了
            MessageBroker.Default.Publish(new InGameReadyMessage());
            // 宝箱を取るまで待つ
            await UniTask.WaitUntil(() => _gameState.IsGetTreasure);
            // ゲームクリアの演出
            await _uiManager.GameClearAnimationAsync(token);

            // 
            // 何か行動したらイベントマネージャーに通知。
            // イベントマネージャーがGameStateに書き込み。
            // どこからでも読み取る。

            // 宝箱に移動
            // 入口に戻ってくる
            //  宝箱のフラグが立っている状態かつ入口
        }

        // ギズモに描画するイベント関数を使用するためにVContainerのエントリポイントを使用せずMonoBehaviorを使う。
        // 専用のクラスを作れば良いがそこまでする必要性。
        void OnDrawGizmos()
        {
            if(_dungeonManager != null) _dungeonManager.DrawOnGizmos();
        }
    }
}
