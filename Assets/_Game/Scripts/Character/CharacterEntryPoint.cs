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
        OpenAiRequest _contextJudgeAi;

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
            _gameRuleAi = new(_gameRule.ToString());
            _characterAi = new(_character.ToString());
            _contextJudgeAi = new(_contextJudge.ToString());
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // ゲームを通して流れる曲
            AudioPlayer.Play(AudioKey.GameBGM, AudioPlayer.PlayMode.BGM);

            while (!token.IsCancellationRequested)
            {
                // プレイヤーの入力もしくはゲームの状態を解析した文章から優先度が最も高いものを選ぶ。
                Talk.Message reaction = _talk.CharacterAI.SelectTopPriorityOption();
                Talk.Message nextAction = _talk.GameRuleAI.SelectTopPriorityOption();

                if (reaction != null && reaction.Text != "")
                {
                    // キャラクターの台詞は投げっぱなし。
                    CharacterLineAsync(reaction.Text).Forget();
                    // 心情の変化も投げっぱなし。
                    ContextJudgeAsync(reaction.Text).Forget();
                }

                if (nextAction != null && nextAction.Text != "")
                {
                    await RequestNextKeyInputAsync(nextAction.Text);
                }
                
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // キャラクター台詞をリクエスト。
        async UniTask CharacterLineAsync(string text)
        {
            string response = await _characterAi.RequestAsync(text);
            _talk.CharacterAI.SetResponse(response);
            _talk.AddLog(_talk.Settings.LogHeader, response);
        }

        // 文脈から心情の変化を行う。
        async UniTask ContextJudgeAsync(string text)
        {
            text = $"次の文章について、印象をルールに基づいて答えてください。'''{text}";

            // 心情の変化量については文脈の判定ルールを記述したテキストファイルに書いてある。
            string response = await _contextJudgeAi.RequestAsync(text);
            if (int.TryParse(response, out int delta))
            {
                _talk.SetMental(_talk.Mental.Value + delta);
            }
            else
            {
                Debug.LogWarning($"心情の変化量にパース出来ない: {response}");
            }
        }

        // ゲームのルールを基に次のキー入力をAIが決定し、メッセージを送信。
        async UniTask RequestNextKeyInputAsync(string text)
        {
            string response = await _gameRuleAi.RequestAsync(text);
            _talk.GameRuleAI.SetResponse(response);

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
    }
}