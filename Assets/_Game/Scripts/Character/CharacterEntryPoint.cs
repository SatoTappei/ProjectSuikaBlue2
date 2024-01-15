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

        GameState _gameState;

        [Inject]
        void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenApiRequest gameRuleApi = new(_gameRule.ToString());
            OpenApiRequest characterApi = new(_character.ToString());
            while (!token.IsCancellationRequested)
            {
                string request = Translator.Translate(_gameState);
                ApiResponseMessage response = await gameRuleApi.RequestAsync(request);
                string command = response.choices[0].message.content;
                InputMessenger.SendMessage(command);
                Debug.Log(command);
                // プレイヤーが行動中かどうかに関わらず一定間隔でリクエストしている
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }

            //while (!token.IsCancellationRequested)
            //{
            //    string request = string.Empty;
            //    if (_player.Forward == PlayerForward.East) request = "あなたは現在東を向いています。";
            //    if (_player.Forward == PlayerForward.West) request = "あなたは現在西を向いています。";
            //    if (_player.Forward == PlayerForward.South) request = "あなたは現在南を向いています。";
            //    if (_player.Forward == PlayerForward.North) request = "あなたは現在北を向いています。";

            //    if (_player.OnFloorBorder) request += "あなたの目の前には大きな穴があります。";

            //    request += "あなたはどの方向に進みますか？目の前に大きな穴がある場合は落ちない方向に進む方角を答えてください。";

            //    // ChatGPTにリクエスト
            //    ApiResponseMessage response = await api.RequestAsync(request);
            //    string line = response.choices[0].message.content;
            //    Debug.Log("じぴて: " + line);

            //    //if (line == "東") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.D });
            //    //if (line == "西") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.A });
            //    //if (line == "南") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.S });
            //    //if (line == "北") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.W });

            //    await UniTask.WaitForSeconds(2.0f, cancellationToken: token);
            //}
        }
    }
}
