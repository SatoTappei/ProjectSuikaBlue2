using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using System.Globalization;
using YamlDotNet.Serialization;

namespace PSB.Game
{
    // このスクリプトがアタッチされたオブジェクトが移動と回転どちらも行う
    public class Player : MonoBehaviour
    {
        [SerializeField] DungeonManager _dungeonManager;

        GameState _gameState;
        PlayerParameterSettings _settings;
        Vector2Int _currentIndex;
        Direction _forward;

        [Inject]
        void Construct(GameState gameState, PlayerParameterSettings settings)
        {
            _gameState = gameState;
            _settings = settings;
        }

        void Start()
        {
            FlowAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // ゲーム開始から操作終了までの一連の流れ
        async UniTaskVoid FlowAsync(CancellationToken token)
        {
            Init();

            // 準備完了のメッセージを受信するまで操作させない
            await MessageAwaiter.ReceiveAsync<InGameReadyMessage>(token);

            await UpdateAsync(token);
        }

        void Init()
        {
            // ダンジョンの入口が初期位置
            transform.position = _dungeonManager.StartPosition();
            transform.Translate(_settings.GroundOffset);
            _currentIndex = _dungeonManager.StartIndex();

            // 回転が0の状態は北向き
            _forward = Direction.North;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // キャンセル時に行動がスキップされる
                using (CancellationTokenSource skipTokenSource = new())
                {
                    // 入力のメッセージが飛んでくるまで待機
                    KeyInputMessage msg = await MessageAwaiter.ReceiveAsync<KeyInputMessage>(token);

                    // 移動もしくは回転
                    if (msg.IsMoveKey(out KeyCode moveKey))
                    {
                        await MoveAsync(moveKey, skipTokenSource.Token);
                    }
                    else if (msg.IsRotateKey(out KeyCode rotKey))
                    {
                        await RotateAsync(rotKey, skipTokenSource.Token);
                    }
                }
            }
        }

        // 移動
        async UniTask MoveAsync(KeyCode key, CancellationToken skipToken)
        {
            // 前後移動のキーか判定、向きに応じた隣のセルを指す方向を取得
            if (!key.TryGetFrontAndBackIndexDirection(_forward, out Vector2Int neighbour)) return;
            // 移動先があるかチェック
            if (!_dungeonManager.TryGetMovablePosition(_currentIndex, neighbour, out Vector3 target)) return;

            // 音を再生
            AudioPlayer.PlayLoop(this, _settings.WalkSeLoop, _settings.WalkSeDelay, 
                AudioKey.WalkStepSE, AudioPlayer.PlayMode.SE);

            // 高さのオフセットを足した移動量を移動
            Vector3 move = target + _settings.GroundOffset - transform.position;
            float speed = _settings.MoveSpeed;
            await transform.MoveAsync(move, speed, skipToken);

            _currentIndex += neighbour;
        }

        // 回転
        async UniTask RotateAsync(KeyCode key, CancellationToken skipToken)
        {
            float rot = key.To90DegreeRotateAngle();
            float speed = _settings.RotateSpeed;
            await transform.RotateAsync(rot, speed, skipToken);
            
            _forward = key.ToTurnedDirection(_forward);
        }
    }
}