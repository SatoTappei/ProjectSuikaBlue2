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
                // �v���C���[�̓��͂𔻒肵�ăC���Q�[�����̑�������߂�
                await ContextJudgeAsync(playerSend, contextJudgeAi, gameRuleAi, token);
                // �C���Q�[�����̃L�����N�^�[���s�������ǂ����Ɋւ�炸���Ԋu�Ń��N�G�X�g���Ă���
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // �v���C���[�̓��͂ɔ��������L�����N�^�[�̑䎌
        async UniTask CharacterTalkAsync(string playerSend, OpenAiRequest character, CancellationToken token)
        {
            if (playerSend == "") return;

            string response = await character.RequestAsync(playerSend);
            // �L�����N�^�[�̑䎌�Ƃ��ăZ�b�g ��b�����ɒǉ�
            _talkState.SetCharacterLine(response);
            _talkState.AddLog(_talkState.Settings.LogHeader, response);

            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // �v���C���[�̓��͂̕����𔻒肵�Ď��̍s�������߂�
        async UniTask ContextJudgeAsync(string playerSend, OpenAiRequest contextJudge, OpenAiRequest gameRule, CancellationToken token)
        {
            if (playerSend == "") return;

            string response = await contextJudge.RequestAsync(playerSend);
            // �S���ύX
            if (int.TryParse(response, out int result)) _talkState.Mental += result;
            // �w��(-1)�Ɣ��f���ꂽ�ꍇ�̓v���C���[�̎w���ɏ]��
            if (result == -1) await PlayerFollowAsync(response, gameRule, token);
            else await GameStateJudgeAsync(gameRule, token);
        }

        // �v���C���[�̎w���ɏ]��
        async UniTask PlayerFollowAsync(string line, OpenAiRequest gameRule, CancellationToken token)
        {
            string request = Translator.Translate(line);
            string response = await gameRule.RequestAsync(request);
            InputMessenger.SendMessage(_gameState, response);
        }

        // �Q�[���̏�Ԃ�API�����f���Ď��̍s�������߂�
        async UniTask GameStateJudgeAsync(OpenAiRequest gameRule, CancellationToken token)
        {
            string request = Translator.Translate(_gameState);
            string response = await gameRule.RequestAsync(request);
            InputMessenger.SendMessage(_gameState, response);
        }
    }
}