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
        OpenAiRequest _contextJudgeAi;

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
            _gameRuleAi = new(_gameRule.ToString());
            _characterAi = new(_character.ToString());
            _contextJudgeAi = new(_contextJudge.ToString());
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // �Q�[����ʂ��ė�����
            AudioPlayer.Play(AudioKey.GameBGM, AudioPlayer.PlayMode.BGM);

            while (!token.IsCancellationRequested)
            {
                // �v���C���[�̓��͂������̓Q�[���̏�Ԃ���͂������͂���D��x���ł��������̂�I�ԁB
                Talk.Message reaction = _talk.CharacterAI.SelectTopPriorityOption();
                Talk.Message nextAction = _talk.GameRuleAI.SelectTopPriorityOption();

                if (reaction != null && reaction.Text != "")
                {
                    // �L�����N�^�[�̑䎌�͓������ςȂ��B
                    CharacterLineAsync(reaction.Text).Forget();
                    // �S��̕ω����������ςȂ��B
                    ContextJudgeAsync(reaction.Text).Forget();
                }

                if (nextAction != null && nextAction.Text != "")
                {
                    await RequestNextKeyInputAsync(nextAction.Text);
                }
                
                await UniTask.WaitForSeconds(_requestInterval, cancellationToken: token);
            }
        }

        // �L�����N�^�[�䎌�����N�G�X�g�B
        async UniTask CharacterLineAsync(string text)
        {
            string response = await _characterAi.RequestAsync(text);
            _talk.CharacterAI.SetResponse(response);
            _talk.AddLog(_talk.Settings.LogHeader, response);
        }

        // ��������S��̕ω����s���B
        async UniTask ContextJudgeAsync(string text)
        {
            text = $"���̕��͂ɂ��āA��ۂ����[���Ɋ�Â��ē����Ă��������B'''{text}";

            // �S��̕ω��ʂɂ��Ă͕����̔��胋�[�����L�q�����e�L�X�g�t�@�C���ɏ����Ă���B
            string response = await _contextJudgeAi.RequestAsync(text);
            if (int.TryParse(response, out int delta))
            {
                _talk.SetMental(_talk.Mental.Value + delta);
            }
            else
            {
                Debug.LogWarning($"�S��̕ω��ʂɃp�[�X�o���Ȃ�: {response}");
            }
        }

        // �Q�[���̃��[������Ɏ��̃L�[���͂�AI�����肵�A���b�Z�[�W�𑗐M�B
        async UniTask RequestNextKeyInputAsync(string text)
        {
            string response = await _gameRuleAi.RequestAsync(text);
            _talk.GameRuleAI.SetResponse(response);

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
    }
}