using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using UniRx;

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
        Talk _talk;
        OpenAiRequest _gameRuleAi;
        OpenAiRequest _characterAi;

        [Inject]
        void Construct(GameState gameState, Talk talk)
        {
            _gameState = gameState;
            _talk = talk;
        }

        void Start()
        {
            if (_useApi)
            {
                Init();
                UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else Debug.LogWarning("OpenAI���g�p���Ȃ���ԂŎ��s��");
        }

        void Init()
        {
            // AI�Ƀ��N�G�X�g����N���X
            _gameRuleAi = new(_gameRule.ToString());
            _characterAi = new(_character.ToString());
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // �Q�[���̏�Ԃ�]������B
                Evaluate();

                // �v���C���[�̓��͂������̓Q�[���̏�Ԃ���͂������͂���D��x���ł��������̂�I�ԁB
                Talk.Message msg = _talk.SelectTopPriorityMessage();
                if (msg == null || msg.Text == "")
                {
                    // ���͂������ꍇ�͎��̃^�C�~���O�܂ő҂B
                    await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
                    continue;
                }

                // �L�����N�^�[�̑䎌�͓������ςȂ��B
                CharacterLineAsync(msg.Text, token).Forget();
                // AI�ɂ��L�[���͂�҂B
                await RequestNextKeyInputAsync(msg.Text, token);

                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // �L�����N�^�[�䎌�����N�G�X�g�B
        async UniTask CharacterLineAsync(string text, CancellationToken token)
        {
            string response = await _characterAi.RequestAsync(text);
            _talk.SetCharacterAiResponse(response);
        }

        // �Q�[���̃��[������Ɏ��̃L�[���͂�AI�����肵�A���b�Z�[�W�𑗐M�B
        async UniTask RequestNextKeyInputAsync(string text, CancellationToken token)
        {
            string response = await _gameRuleAi.RequestAsync(text);
            _talk.SetGameRuleAiResponse(response);

            // AI����̃��X�|���X����ɃL�[���͂̃��b�Z�[�W�𑗐M�B
            // �Q�[�����[�����L�q�����e�L�X�g�t�@�C����AI���̃��X�|���X�̃��[���������Ă���B
            KeyInputMessage msg = new();
            if (response.Contains("1")) msg.KeyDownW = true;
            else if (response.Contains("2")) msg.KeyDownS = true;
            else if (response.Contains("3")) msg.KeyDownA = true;
            else if (response.Contains("4")) msg.KeyDownD = true;
            else return; // �ǂ�ɂ��Y�����Ȃ��ꍇ�͑��M���Ȃ��B

            MessageBroker.Default.Publish(msg);
        }

        // �Q�[���̏�Ԃ����AI�ւ̃��N�G�X�g���쐬�B
        void Evaluate()
        {
            string f = $"�O��{_gameState.ForwardEvaluate}���i�ނ��Ƃ��o���܂��B";
            string b = $"����{_gameState.BackEvaluate}���i�ނ��Ƃ��o���܂��B";
            string l = $"����������{_gameState.LeftEvaluate}���i�߂铹������܂��B";
            string r = $"�E��������{_gameState.RightEvaluate}���i�߂铹������܂��B";

            _talk.AddMessage(f + b + l + r + "�ǂ̕����ɐi�݂܂����H", _talk.Mental);
        }
    }
}