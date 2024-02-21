using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Text;

namespace PSB.Game
{
    /// <summary>
    /// �Q�[���̏�Ԃ𕶏͉�����AI�ւ̃��N�G�X�g����ǉ��B
    /// </summary>
    public class Translator : MonoBehaviour
    {
        // �����C�x���g2��ڈȍ~�̗D��x
        // ���N�G�X�g�ɑI�΂�Ȃ��悤�ȒႢ�l�B
        const int _secondPriority = -1;

        [Header("�X�V�Ԋu")]
        [SerializeField] float _interval = 1.0f;

        IReadOnlyGameState _gameState;
        Talk _talk;

        HashSet<Vector2Int> _visited = new();
        // �Ō�Ƀ_���[�W���󂯂����ԂƂ̍��B
        float _lastDamagedTime;
        // ���̏�Ԃ�1�x���������m����t���O�B
        bool _firstInGameReady = true;
        bool _firstGetTreasure = true;
        bool _firstInGameClear = true;

        [Inject]
        void Construct(GameState gameState, Talk talk)
        {
            _gameState = gameState;
            _talk = talk;
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                AddGameStateRequests();
                await UniTask.WaitForSeconds(_interval, cancellationToken: token);
            }
        }

        // �Q�[���̏�Ԃ�����
        void AddGameStateRequests()
        {
            // �ړ��悪���ɖK�ꂽ�ӏ����ǂ����𔻒肷�邽�߂ɕK�v
            _visited.Add(_gameState.PlayerIndex);

            // �Q�[���J�n�O
            if (!_gameState.IsInGameReady)
            {
                string s = "�Q�[�����n�߂�O�A�ӋC���݂������Ă��������B";
                int p = _firstInGameReady ? _talk.Settings.InGameEventPriority : _secondPriority;
                _talk.CharacterAI.AddOption(s, p, Talk.Owner.GameState);
                _firstInGameReady = false;
            }

            // �������
            if (_gameState.IsGetTreasure)
            {
                string s = "���Ȃ��͂������ɓ���܂����B";
                int p = _firstGetTreasure ? _talk.Settings.InGameEventPriority : _secondPriority;
                _talk.CharacterAI.AddOption(s, p, Talk.Owner.GameState);
                _firstGetTreasure = false;
            }

            // �Q�[���N���A
            if (_gameState.IsInGameClear)
            {
                string s = "���Ȃ��̓Q�[�����N���A���܂����B";
                int p = _firstInGameClear ? _talk.Settings.InGameEventPriority : _secondPriority;
                _talk.CharacterAI.AddOption(s, p, Talk.Owner.GameState);
                _firstInGameClear = false;
            }

            // ���ꂩ�̕����ɐi��
            {
                string s = $"�O��{_gameState.ForwardEvaluate}���i�ނ��Ƃ��o���܂��B'''" +
                    $"����{_gameState.BackEvaluate}���i�ނ��Ƃ��o���܂��B'''" +
                    $"����������{_gameState.LeftEvaluate}���i�߂铹������܂��B'''" +
                    $"�E��������{_gameState.RightEvaluate}���i�߂铹������܂��B'''" +
                    $"�ǂ̕����ɐi�݂܂����H";
                _talk.GameRuleAI.AddOption(s, _talk.Mental.Value, Talk.Owner.GameState);

                string t = "���Ȃ��͈ړ����s���܂��B'''���A�N�V���������Ă��������B";
                _talk.CharacterAI.AddOption(t, _talk.Mental.Value, Talk.Owner.GameState);
            }

            // �_���[�W���󂯂�
            if (!Mathf.Approximately(_gameState.LastDamagedTime, _lastDamagedTime))
            {
                string s = "�_���[�W���󂯂��Ƃ��̃��A�N�V���������Ă��������B";
                _talk.CharacterAI.AddOption(s, _talk.Settings.DamagedPriority, Talk.Owner.GameState);

                _lastDamagedTime = _gameState.LastDamagedTime;
            }
        }
    }
}
