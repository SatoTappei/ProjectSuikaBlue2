using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] TextAsset _gameRule;
        [SerializeField] PlayerController _player;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            Debug.Log(_gameRule.ToString());
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenApiRequest api = new(_gameRule.ToString());
            while (!token.IsCancellationRequested)
            {
                string request = string.Empty;
                if (_player.Forward == PlayerForward.East) request = "���Ȃ��͌��ݓ��������Ă��܂��B";
                if (_player.Forward == PlayerForward.West) request = "���Ȃ��͌��ݐ��������Ă��܂��B";
                if (_player.Forward == PlayerForward.South) request = "���Ȃ��͌��ݓ�������Ă��܂��B";
                if (_player.Forward == PlayerForward.North) request = "���Ȃ��͌��ݖk�������Ă��܂��B";

                if (_player.OnFloorBorder) request += "���Ȃ��̖ڂ̑O�ɂ͑傫�Ȍ�������܂��B";

                request += "���Ȃ��͂ǂ̕����ɐi�݂܂����H�ڂ̑O�ɑ傫�Ȍ�������ꍇ�͗����Ȃ������ɐi�ޕ��p�𓚂��Ă��������B";

                // ChatGPT�Ƀ��N�G�X�g
                ApiResponseMessage response = await api.RequestAsync(request);
                string line = response.choices[0].message.content;
                Debug.Log("���҂�: " + line);

                if (line == "��") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.D });
                if (line == "��") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.A });
                if (line == "��") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.S });
                if (line == "�k") MessageBroker.Default.Publish(new PlayerControlMessage() { Key = KeyCode.W });

                await UniTask.WaitForSeconds(2.0f, cancellationToken: token);
            }
        }
    }
}
