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
        [Header("OpenAPI�ւ̃��N�G�X�g�ݒ�")]
        [SerializeField] float _requestInterval = 2.0f;
        [Header("�f�o�b�O�p: OpenApi���g�p���邩")]
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
            else Debug.LogWarning("OpenAPI���g�p���Ȃ���ԂŎ��s��");
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenApiRequest gameRuleApi = new(_gameRule.ToString());
            OpenApiRequest characterApi = new(_character.ToString());
            while (!token.IsCancellationRequested)
            {
                // �Q�[���̏�Ԃ�API�����f���Ď��̍s�������߂�
                string request = Translator.Translate(_gameState);
                ApiResponseMessage response = await gameRuleApi.RequestAsync(request);
                string command = response.choices[0].message.content;
                InputMessenger.SendMessage(_gameState, command);

                Debug.Log(command);
                // �v���C���[���s�������ǂ����Ɋւ�炸���Ԋu�Ń��N�G�X�g���Ă���
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }
    }
}
