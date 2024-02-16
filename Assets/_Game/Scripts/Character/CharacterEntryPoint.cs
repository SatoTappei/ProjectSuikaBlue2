using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using UniRx;

namespace PSB.Game
{
    public class CharacterEntryPoint : MonoBehaviour
    {
        [SerializeField] TextAsset _gameRule;
        [SerializeField] TextAsset _character;
        [SerializeField] TextAsset _contextJudge;
        [Header("OpenAIへのリクエスト設定")]
        [SerializeField] float _requestInterval = 2.0f;
        [Header("デバッグ用: OpenAIを使用するか")]
        [SerializeField] bool _useApi;

        IReadOnlyGameState _gameState;
        Talk _talk;
        OpenAiRequest _gameRuleAi;
        OpenAiRequest _characterAi;

        [Inject]
        void Construct(GameState gameState, Talk talk)
        {
            _gameState = gameState;
            _talk = talk;
        }

        void Start()
        {
            if (_useApi)
            {
                Init();
                UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else Debug.LogWarning("OpenAIを使用しない状態で実行中");
        }

        void Init()
        {
            // AIにリクエストするクラス
            _gameRuleAi = new(_gameRule.ToString());
            _characterAi = new(_character.ToString());
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // ゲームの状態を評価する。
                Evaluate();

                // プレイヤーの入力もしくはゲームの状態を解析した文章から優先度が最も高いものを選ぶ。
                Talk.Message msg = _talk.SelectTopPriorityMessage();
                if (msg == null || msg.Text == "")
                {
                    // 入力が無い場合は次のタイミングまで待つ。
                    await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
                    continue;
                }

                // キャラクターの台詞は投げっぱなし。
                CharacterLineAsync(msg.Text, token).Forget();
                // AIによるキー入力を待つ。
                await RequestNextKeyInputAsync(msg.Text, token);

                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // キャラクター台詞をリクエスト。
        async UniTask CharacterLineAsync(string text, CancellationToken token)
        {
            string response = await _characterAi.RequestAsync(text);
            _talk.SetCharacterAiResponse(response);
        }

        // ゲームのルールを基に次のキー入力をAIが決定し、メッセージを送信。
        async UniTask RequestNextKeyInputAsync(string text, CancellationToken token)
        {
            string response = await _gameRuleAi.RequestAsync(text);
            _talk.SetGameRuleAiResponse(response);

            // AIからのレスポンスを基にキー入力のメッセージを送信。
            // ゲームルールを記述したテキストファイルにAI側のレスポンスのルールが書いてある。
            KeyInputMessage msg = new();
            if (response.Contains("1")) msg.KeyDownW = true;
            else if (response.Contains("2")) msg.KeyDownS = true;
            else if (response.Contains("3")) msg.KeyDownA = true;
            else if (response.Contains("4")) msg.KeyDownD = true;
            else return; // どれにも該当しない場合は送信しない。

            MessageBroker.Default.Publish(msg);
        }

        // ゲームの状態を基にAIへのリクエストを作成。
        void Evaluate()
        {
            string f = $"前に{_gameState.ForwardEvaluate}歩進むことが出来ます。";
            string b = $"後ろに{_gameState.BackEvaluate}歩進むことが出来ます。";
            string l = $"左を向くと{_gameState.LeftEvaluate}歩進める道があります。";
            string r = $"右を向くと{_gameState.RightEvaluate}歩進める道があります。";

            _talk.AddMessage(f + b + l + r + "どの方向に進みますか？", _talk.Mental);
        }
    }
}