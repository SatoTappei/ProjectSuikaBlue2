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
        [Header("OpenAI�ւ̃��N�G�X�g�ݒ�")]
        [SerializeField] float _requestInterval = 2.0f;
        [Header("�f�o�b�O�p: OpenAI���g�p���邩")]
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
            else Debug.LogWarning("OpenAI���g�p���Ȃ���ԂŎ��s��");
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenAiRequest gameRuleAi = new(_gameRule.ToString());
            OpenAiRequest characterAi = new(_character.ToString());
            OpenAiRequest contextJudgeAi = new(_contextJudge.ToString());
            while (!token.IsCancellationRequested)
            {
                string playerSend = _talkState.GetPlayerSend();
                // �L�����N�^�[�̑䎌��UI�ɕ\�����邾���Ȃ̂ő҂K�v�Ȃ�
                CharacterTalkAsync(playerSend, characterAi, token).Forget();
                ContextJudgeAsync(playerSend, contextJudgeAi, token).Forget();
                await NextActionAsync(gameRuleAi, token);

                // �v���C���[���s�������ǂ����Ɋւ�炸���Ԋu�Ń��N�G�X�g���Ă���
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // �v���C���[�̓��͂ɔ��������L�����N�^�[�̑䎌
        async UniTask CharacterTalkAsync(string playerSend, OpenAiRequest api, CancellationToken token)
        {
            if (playerSend == "") return;

            ApiResponseMessage response = await api.RequestAsync(playerSend);
            string line = response.choices[0].message.content;

            _talkState.SetCharacterLine(line);
            _talkState.AddLog(_talkState.Settings.LogHeader, line);

            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // �v���C���[�̓��͂̕����𔻒�
        async UniTask ContextJudgeAsync(string playerSend, OpenAiRequest api, CancellationToken token)
        {
            if (playerSend == "") return;

            ApiResponseMessage response = await api.RequestAsync(playerSend);
            string line = response.choices[0].message.content;

            Debug.Log(line);
        }

        // �Q�[���̏�Ԃ�API�����f���Ď��̍s�������߂�
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

// �䎌��Hot��Cold������B
// �Z���Ԋu�Ŏw�������N�G�X�g����Ɠ{��B
// ���l��S��ɔ��f������B
// �w���������ꍇ�́A�����AI�̃v���C�ɔ��f������B