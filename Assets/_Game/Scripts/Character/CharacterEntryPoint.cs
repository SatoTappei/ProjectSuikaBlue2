using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using VContainer.Unity;

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

        IReadOnlyGameState _gameState;

        [Inject]
        void Construct(GameState gameState)
        {
            _gameState = gameState;
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
                // ゲームの状態をAPIが判断して次の行動を決める
                string request = Translator.Translate(_gameState);
                ApiResponseMessage response = await gameRuleApi.RequestAsync(request);
                string command = response.choices[0].message.content;
                InputMessenger.SendMessage(_gameState, command);

                Debug.Log(command);
                // プレイヤーが行動中かどうかに関わらず一定間隔でリクエストしている
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }
    }
}
