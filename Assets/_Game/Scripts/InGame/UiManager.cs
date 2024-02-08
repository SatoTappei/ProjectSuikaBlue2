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
        [Header("現在の目標")]
        [SerializeField] Text _nextGoalText;
        [SerializeField] string _chestNextGoal;
        [SerializeField] string _returnNextGoal;
        [SerializeField] float _textAnimationInterval = 0.25f;
        [Header("ゲームクリア画面")]
        [SerializeField] GameObject _gameClear;
        [SerializeField] Text _gameClearText;
        [SerializeField] float _gameClearAnimationDuration = 1.0f;

        GameState _gameState;
        // テキストの表示アニメーション用
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

        // 開始時にUIを非表示にする
        void DisableUI()
        {
            _gameClear.SetActive(false);
            _gameClearText.text = "";
        }

        // フラグを監視して目標を更新する
        void NextGoal()
        {
            // 宝箱を取る目標
            TextAnimation(_chestNextGoal);
            // 入口まで戻る目標
            _gameState.ObserveEveryValueChanged(g => g.IsGetTreasure).Where(b => b).Subscribe(_ =>
            {
                TextAnimation(_returnNextGoal);
            });
        }

        // 1文字ずつ表示する
        void TextAnimation(string s)
        {
            ReleaseCTS();

            _textAnimationSource = new();
            TextAnimationAsync(_nextGoalText, _textAnimationInterval, s, _textAnimationSource.Token).Forget();
        }

        // 1文字ずつ表示するアニメーション
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
        /// ゲームクリア時のアニメーション
        /// </summary>
        public async UniTask GameClearAnimationAsync(CancellationToken token)
        {
            await GameClearAnimationAsync(_gameClearAnimationDuration, token);
        }

        // パネルを表示してテキストをアニメーション
        async UniTask GameClearAnimationAsync(float duration, CancellationToken token)
        {
            _gameClear.SetActive(true);
            await UniTask.Yield(token);
            // GameClearの文字数が9文字
            await TextAnimationAsync(_gameClearText, duration / 9, "GameClear", token);
        }
    }
}