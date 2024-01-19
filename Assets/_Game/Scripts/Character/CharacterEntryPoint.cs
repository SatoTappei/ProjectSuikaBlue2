using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;

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
        TalkState _talkState;

        [Inject]
        void Construct(GameState gameState, TalkState talkState)
        {
            _gameState = gameState;
            _talkState = talkState;
        }

        void Start()
        {
            if (_useApi) UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            else Debug.LogWarning("OpenAIを使用しない状態で実行中");
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenAiRequest gameRuleAi = new(_gameRule.ToString());
            OpenAiRequest characterAi = new(_character.ToString());
            OpenAiRequest contextJudgeAi = new(_contextJudge.ToString());
            while (!token.IsCancellationRequested)
            {
                string playerSend = _talkState.GetPlayerSend();
                // キャラクターの台詞はUIに表示するだけなので待つ必要なし
                CharacterTalkAsync(playerSend, characterAi, token).Forget();
                // プレイヤーの入力を判定してインゲーム内の操作を決める
                await ContextJudgeAsync(playerSend, contextJudgeAi, gameRuleAi, token);
                // インゲーム内のキャラクターが行動中かどうかに関わらず一定間隔でリクエストしている
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // プレイヤーの入力に反応したキャラクターの台詞
        async UniTask CharacterTalkAsync(string playerSend, OpenAiRequest character, CancellationToken token)
        {
            if (playerSend == "") return;

            string response = await character.RequestAsync(playerSend);
            // キャラクターの台詞としてセット 会話履歴に追加
            _talkState.SetCharacterLine(response);
            _talkState.AddLog(_talkState.Settings.LogHeader, response);

            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // プレイヤーの入力の文脈を判定して次の行動を決める
        async UniTask ContextJudgeAsync(string playerSend, OpenAiRequest contextJudge, OpenAiRequest gameRule, CancellationToken token)
        {
            if (playerSend == "") return;

            string response = await contextJudge.RequestAsync(playerSend);
            // 心情を変更
            if (int.TryParse(response, out int result)) _talkState.Mental += result;
            // 指示(-1)と判断された場合はプレイヤーの指示に従う
            if (result == -1) await PlayerFollowAsync(response, gameRule, token);
            else await GameStateJudgeAsync(gameRule, token);
        }

        // プレイヤーの指示に従う
        async UniTask PlayerFollowAsync(string line, OpenAiRequest gameRule, CancellationToken token)
        {
            string request = Translator.Translate(line);
            string response = await gameRule.RequestAsync(request);
            InputMessenger.SendMessage(_gameState, response);
        }

        // ゲームの状態をAPIが判断して次の行動を決める
        async UniTask GameStateJudgeAsync(OpenAiRequest gameRule, CancellationToken token)
        {
            string request = Translator.Translate(_gameState);
            string response = await gameRule.RequestAsync(request);
            InputMessenger.SendMessage(_gameState, response);
        }
    }
}