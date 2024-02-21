using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Text;

namespace PSB.Game
{
    /// <summary>
    /// ゲームの状態を文章化してAIへのリクエスト候補を追加。
    /// </summary>
    public class Translator : MonoBehaviour
    {
        // 同じイベント2回目以降の優先度
        // リクエストに選ばれないような低い値。
        const int _secondPriority = -1;

        [Header("更新間隔")]
        [SerializeField] float _interval = 1.0f;

        IReadOnlyGameState _gameState;
        Talk _talk;

        HashSet<Vector2Int> _visited = new();
        // 最後にダメージを受けた時間との差。
        float _lastDamagedTime;
        // その状態で1度だけを検知するフラグ。
        bool _firstInGameReady = true;
        bool _firstGetTreasure = true;
        bool _firstInGameClear = true;

        [Inject]
        void Construct(GameState gameState, Talk talk)
        {
            _gameState = gameState;
            _talk = talk;
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                AddGameStateRequests();
                await UniTask.WaitForSeconds(_interval, cancellationToken: token);
            }
        }

        // ゲームの状態を見る
        void AddGameStateRequests()
        {
            // 移動先が既に訪れた箇所かどうかを判定するために必要
            _visited.Add(_gameState.PlayerIndex);

            // ゲーム開始前
            if (!_gameState.IsInGameReady)
            {
                string s = "ゲームを始める前、意気込みを言ってください。";
                int p = _firstInGameReady ? _talk.Settings.InGameEventPriority : _secondPriority;
                _talk.CharacterAI.AddOption(s, p, Talk.Owner.GameState);
                _firstInGameReady = false;
            }

            // お宝入手
            if (_gameState.IsGetTreasure)
            {
                string s = "あなたはお宝を手に入れました。";
                int p = _firstGetTreasure ? _talk.Settings.InGameEventPriority : _secondPriority;
                _talk.CharacterAI.AddOption(s, p, Talk.Owner.GameState);
                _firstGetTreasure = false;
            }

            // ゲームクリア
            if (_gameState.IsInGameClear)
            {
                string s = "あなたはゲームをクリアしました。";
                int p = _firstInGameClear ? _talk.Settings.InGameEventPriority : _secondPriority;
                _talk.CharacterAI.AddOption(s, p, Talk.Owner.GameState);
                _firstInGameClear = false;
            }

            // 何れかの方向に進む
            {
                string s = $"前に{_gameState.ForwardEvaluate}歩進むことが出来ます。'''" +
                    $"後ろに{_gameState.BackEvaluate}歩進むことが出来ます。'''" +
                    $"左を向くと{_gameState.LeftEvaluate}歩進める道があります。'''" +
                    $"右を向くと{_gameState.RightEvaluate}歩進める道があります。'''" +
                    $"どの方向に進みますか？";
                _talk.GameRuleAI.AddOption(s, _talk.Mental.Value, Talk.Owner.GameState);

                string t = "あなたは移動を行います。'''リアクションをしてください。";
                _talk.CharacterAI.AddOption(t, _talk.Mental.Value, Talk.Owner.GameState);
            }

            // ダメージを受けた
            if (!Mathf.Approximately(_gameState.LastDamagedTime, _lastDamagedTime))
            {
                string s = "ダメージを受けたときのリアクションをしてください。";
                _talk.CharacterAI.AddOption(s, _talk.Settings.DamagedPriority, Talk.Owner.GameState);

                _lastDamagedTime = _gameState.LastDamagedTime;
            }
        }
    }
}
