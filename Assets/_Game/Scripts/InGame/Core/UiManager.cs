using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PSB.Game
{
    public class UiManager : MonoBehaviour
    {
        [Header("���݂̖ڕW")]
        [SerializeField] Text _nextGoalText;
        [SerializeField] string _chestNextGoal;
        [SerializeField] string _returnNextGoal;
        [SerializeField] float _textAnimationInterval = 0.25f;
        [Header("�Q�[���N���A���")]
        [SerializeField] GameObject _gameClear;
        [SerializeField] Text _gameClearText;
        [SerializeField] float _gameClearAnimationDuration = 1.0f;

        GameState _gameState;
        // �e�L�X�g�̕\���A�j���[�V�����p
        CancellationTokenSource _textAnimationSource;

        [Inject]
        void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        void Awake()
        {
            NextGoal();
            DisableUI();
        }

        void OnDestroy()
        {
            ReleaseCTS();
        }

        // �J�n����UI���\���ɂ���
        void DisableUI()
        {
            _gameClear.SetActive(false);
            _gameClearText.text = "";
        }

        // �t���O���Ď����ĖڕW���X�V����
        void NextGoal()
        {
            // �󔠂����ڕW
            TextAnimation(_chestNextGoal);
            // �����܂Ŗ߂�ڕW
            _gameState.ObserveEveryValueChanged(g => g.IsGetTreasure).Where(b => b).Subscribe(_ =>
            {
                TextAnimation(_returnNextGoal);
            });
        }

        // 1�������\������
        void TextAnimation(string s)
        {
            ReleaseCTS();

            _textAnimationSource = new();
            TextAnimationAsync(_nextGoalText, _textAnimationInterval, s, _textAnimationSource.Token).Forget();
        }

        // 1�������\������A�j���[�V����
        async UniTask TextAnimationAsync(Text text, float interval, string s, CancellationToken token)
        {
            text.text = "";

            int index = 0;
            float elapsed = 0;
            while (index < s.Length && !token.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                if (elapsed > interval)
                {
                    elapsed = 0;
                    text.text += s[index++];
                }

                await UniTask.Yield(token);
            }

            text.text = s;
        }

        void ReleaseCTS()
        {
            _textAnimationSource?.Cancel();
            _textAnimationSource?.Dispose();
        }

        /// <summary>
        /// �Q�[���N���A���̃A�j���[�V����
        /// </summary>
        public async UniTask GameClearAnimationAsync(CancellationToken token)
        {
            await GameClearAnimationAsync(_gameClearAnimationDuration, token);
        }

        // �p�l����\�����ăe�L�X�g���A�j���[�V����
        async UniTask GameClearAnimationAsync(float duration, CancellationToken token)
        {
            _gameClear.SetActive(true);
            await UniTask.Yield(token);
            // GameClear�̕�������9����
            await TextAnimationAsync(_gameClearText, duration / 9, "GameClear", token);
        }
    }
}