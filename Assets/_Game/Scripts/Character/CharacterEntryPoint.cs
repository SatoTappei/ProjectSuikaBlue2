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
        [Header("OpenAPIへのリクエスト設定")]
        [SerializeField] float _requestInterval = 2.0f;
        [Header("デバッグ用: OpenApiを使用するか")]
        [SerializeField] bool _useApi;
        [Header("キャラクター側のログの名前")]
        [SerializeField] string _logHeader = "めいど: ";

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
            else Debug.LogWarning("OpenAPIを使用しない状態で実行中");
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenApiRequest gameRuleApi = new(_gameRule.ToString());
            OpenApiRequest characterApi = new(_character.ToString());
            while (!token.IsCancellationRequested)
            {
                // キャラクターの台詞はUIに表示するだけなので待つ必要なし
                CharacterTalkAsync(characterApi, token).Forget();

                await NextActionAsync(gameRuleApi, token);

                // プレイヤーが行動中かどうかに関わらず一定間隔でリクエストしている
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // プレイヤーの入力を取得
        async UniTask CharacterTalkAsync(OpenApiRequest api, CancellationToken token)
        {
            string send = _talkState.GetPlayerSend();
            if (send == "") return;

            ApiResponseMessage response = await api.RequestAsync(send);
            string line = response.choices[0].message.content;
            _talkState.SetCharacterLine(line);
            _talkState.AddLog(_logHeader, line);

            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // ゲームの状態をAPIが判断して次の行動を決める
        async UniTask NextActionAsync(OpenApiRequest api, CancellationToken token)
        {
            string request = Translator.Translate(_gameState);
            ApiResponseMessage response = await api.RequestAsync(request);
            string command = response.choices[0].message.content;
            InputMessenger.SendMessage(_gameState, command);

            Debug.Log(command);
        }
    }
}