using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;

namespace PSB.Game
{
    public class Player : MonoBehaviour, ICharacter
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
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
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
            transform.position = _dungeonManager.GetStartCell().Position;
            transform.Translate(_settings.GroundOffset);
            _currentIndex = _dungeonManager.GetStartCell().Index;
            _gameState.PlayerIndex = _currentIndex;

            // グリッド上での位置
            _dungeonManager.SetCharacter(CharacterKey.Player, _currentIndex, this);

            // 回転が0の状態は北向き
            transform.rotation = Quaternion.identity;
            _forward = Direction.North;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 準備完了のフラグが立つまで操作させない
            await UniTask.WaitUntil(() => _gameState.IsInGameReady, cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                // 周囲を調べる
                SearchAround();

                // 入力のメッセージが飛んでくるまで待機
                KeyInputMessage msg = await MessageAwaiter.ReceiveAsync<KeyInputMessage>(token);

                // ゲームのクリア条件を満たしていた場合は弾く
                if (_gameState.IsInGameClear) continue;

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

                await UniTask.Yield(token);
            }
        }

        // 移動
        async UniTask MoveAsync(KeyCode key, CancellationToken token)
        {
            // 前後移動のキーか判定、向きに応じた隣のセルを指す方向を取得
            if (!key.TryGetFrontAndBackIndex(_forward, out Vector2Int direction)) return;
            // 移動先があるかチェック
            Vector2Int neighbour = _currentIndex + direction;
            if (!_dungeonManager.IsConnected(_currentIndex, neighbour)) return;

            // 音を再生
            AudioPlayer.PlayLoop(this, _settings.WalkSeLoop, _settings.WalkSeDelay, 
                AudioKey.WalkStepSE, AudioPlayer.PlayMode.SE);

            // 高さのオフセットを足した移動量を移動
            Vector3 move = _dungeonManager.GetCell(neighbour).Position + _settings.GroundOffset - transform.position;
            float speed = _settings.MoveSpeed;
            await transform.MoveAsync(move, speed, token);

            // 現在のグリッド上での位置を削除
            _dungeonManager.SetCharacter(CharacterKey.Dummy, _currentIndex, null);

            _currentIndex = neighbour;
            _gameState.PlayerIndex = _currentIndex;

            // グリッド上での位置を更新
            _dungeonManager.SetCharacter(CharacterKey.Player, _currentIndex, this);
        }

        // 回転
        // このスクリプトがアタッチされたオブジェクト自体が回転する
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
            IReadOnlyCell cell = _dungeonManager.GetCell(_currentIndex);

            // 現在位置が宝箱なら獲得
            if (cell.LocationKey == LocationKey.Chest)
            {
                _gameState.IsGetTreasure = true;
            }

            // 現在地に応じたイベント発生
            cell.Location?.Action();
        }

        // AIが現在の状況を把握するため、周囲を調べてGameStateに書き込む
        void SearchAround()
        {
            // 現在のセルの位置を基準に眼の高さから
            Vector3 p = _dungeonManager.GetCell(_currentIndex).Position;
            p.y = _settings.EyeHeight;

            // 前後左右の進める距離をチェック
            _gameState.ForwardEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Up).ToIndex());
            _gameState.BackEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Down).ToIndex());
            _gameState.LeftEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Left).ToIndex());
            _gameState.RightEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Right).ToIndex());

            // その方向にどれだけ進めるか
            int CheckDistance(Vector2Int index)
            {
                for (int i = 1; ; i++)
                {
                    if (!_dungeonManager.IsConnected(_currentIndex + index * i, _currentIndex + index * (i - 1)))
                    {
                        return i - 1;
                    }
                }
            }
        }

        // ダメージ
        void IDamageReceiver.Damage()
        {
            _gameState.LastDamagedTime = Time.time;

            AudioPlayer.Play(AudioKey.KickDamageSE, AudioPlayer.PlayMode.SE);
            CameraController.Shake();
        }
    }
}