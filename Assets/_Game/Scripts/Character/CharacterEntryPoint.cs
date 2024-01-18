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
        [Header("OpenAPI�ւ̃��N�G�X�g�ݒ�")]
        [SerializeField] float _requestInterval = 2.0f;
        [Header("�f�o�b�O�p: OpenApi���g�p���邩")]
        [SerializeField] bool _useApi;
        [Header("�L�����N�^�[���̃��O�̖��O")]
        [SerializeField] string _logHeader = "�߂���: ";

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
            else Debug.LogWarning("OpenAPI���g�p���Ȃ���ԂŎ��s��");
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenApiRequest gameRuleApi = new(_gameRule.ToString());
            OpenApiRequest characterApi = new(_character.ToString());
            while (!token.IsCancellationRequested)
            {
                // �L�����N�^�[�̑䎌��UI�ɕ\�����邾���Ȃ̂ő҂K�v�Ȃ�
                CharacterTalkAsync(characterApi, token).Forget();

                await NextActionAsync(gameRuleApi, token);

                // �v���C���[���s�������ǂ����Ɋւ�炸���Ԋu�Ń��N�G�X�g���Ă���
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // �v���C���[�̓��͂��擾
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

        // �Q�[���̏�Ԃ�API�����f���Ď��̍s�������߂�
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