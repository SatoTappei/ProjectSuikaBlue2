using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using System.Globalization;
using YamlDotNet.Serialization;
using Unity.VisualScripting;

namespace PSB.Game
{
    // このスクリプトがアタッチされたオブジェクトが移動と回転どちらも行う
    public class Player : MonoBehaviour
    {
        GameState _gameState;
        PlayerParameterSettings _settings;
        DungeonManager _dungeonManager;
        Vector2Int _currentIndex;
        Direction _forward;

        [Inject]
        void Construct(GameState gameState, PlayerParameterSettings settings, DungeonManager dungeonManager)
        {
            _gameState = gameState;
            _settings = settings;
            _dungeonManager = dungeonManager;
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
            // 当たり判定用に使う用に設定
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.mass = 0;
            rb.angularDrag = 0;

            // ダンジョンの入口が初期位置
            transform.position = _dungeonManager.StartPosition();
            transform.Translate(_settings.GroundOffset);
            _currentIndex = _dungeonManager.StartIndex();
            _gameState.CurrentIndex = _currentIndex;

            // 回転が0の状態は北向き
            transform.rotation = Quaternion.identity;
            _forward = Direction.North;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // 入力のメッセージが飛んでくるまで待機
                KeyInputMessage msg = await MessageAwaiter.ReceiveAsync<KeyInputMessage>(token);

                // 移動もしくは回転
                if (msg.IsMoveKey(out KeyCode moveKey))
                {
                    await MoveAsync(moveKey, token);
                }
                else if (msg.IsRotateKey(out KeyCode rotKey))
                {
                    await RotateAsync(rotKey, token);
                }

                // 行動結果
                ActionResult();
            }
        }

        // 移動
        async UniTask MoveAsync(KeyCode key, CancellationToken token)
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
            await transform.MoveAsync(move, speed, token);

            _currentIndex += neighbour;
            _gameState.CurrentIndex = _currentIndex;
        }

        // 回転
        async UniTask RotateAsync(KeyCode key, CancellationToken token)
        {
            float rot = key.To90DegreeRotateAngle();
            float speed = _settings.RotateSpeed;
            await transform.RotateAsync(rot, speed, token);
            
            _forward = key.ToTurnedDirection(_forward);
        }

        // 行動した結果どうなったか
        void ActionResult()
        {
            // 現在位置が宝箱なら獲得
            LocationKey location = _dungeonManager.GetLocation(_currentIndex);
            Debug.Log("ここは" + location);
            if (location == LocationKey.Chest)
            {
                _gameState.IsGetTreasure = true;
            }
        }
    }
}