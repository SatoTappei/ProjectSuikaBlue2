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
                ContextJudgeAsync(playerSend, contextJudgeAi, token).Forget();
                await NextActionAsync(gameRuleAi, token);

                // プレイヤーが行動中かどうかに関わらず一定間隔でリクエストしている
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // プレイヤーの入力に反応したキャラクターの台詞
        async UniTask CharacterTalkAsync(string playerSend, OpenAiRequest api, CancellationToken token)
        {
            if (playerSend == "") return;

            ApiResponseMessage response = await api.RequestAsync(playerSend);
            string line = response.choices[0].message.content;

            _talkState.SetCharacterLine(line);
            _talkState.AddLog(_talkState.Settings.LogHeader, line);

            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // プレイヤーの入力の文脈を判定
        async UniTask ContextJudgeAsync(string playerSend, OpenAiRequest api, CancellationToken token)
        {
            if (playerSend == "") return;

            ApiResponseMessage response = await api.RequestAsync(playerSend);
            string line = response.choices[0].message.content;

            Debug.Log(line);
        }

        // ゲームの状態をAPIが判断して次の行動を決める
        async UniTask NextActionAsync(OpenAiRequest api, CancellationToken token)
        {
            string request = Translator.Translate(_gameState);
            ApiResponseMessage response = await api.RequestAsync(request);
            string command = response.choices[0].message.content;
            InputMessenger.SendMessage(_gameState, command);

            //Debug.Log(command);
        }
    }
}

// 台詞にHotとColdがある。
// 短い間隔で指示をリクエストすると怒る。
// 数値を心情に反映させる。
// 指示だった場合は、それをAIのプレイに反映させる。