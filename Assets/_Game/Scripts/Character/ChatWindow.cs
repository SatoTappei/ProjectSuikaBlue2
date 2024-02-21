using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using UniRx;
using UniRx.Triggers;
using System.Text;

namespace PSB.Game
{
    public class ChatWindow : MonoBehaviour
    {
        [Header("キャラクターとの会話")]
        [SerializeField] InputField _inputField;
        [SerializeField] Button _enterButton;
        [SerializeField] Text _logText;
        [SerializeField] string _logHeader = "あなた: ";
        [Header("チャット画面の開閉")]
        [SerializeField] Button _switchButton;
        [SerializeField] Transform _window;
        [SerializeField] float _closedWidth = -429.0f;
        [SerializeField] float _OpenedWidth = 19.0f;
        [SerializeField] float _animationSpeed = 1.0f;
        [Header("開く閉じるボタンの文字を変える")]
        [SerializeField] string _closedLetter = ">";
        [SerializeField] string _opendLetter = "<";

        StringBuilder _builder = new();
        Talk _talk;
        CancellationToken _token;
        bool _isOpened;

        [Inject]
        void Construct(Talk talk)
        {
            _talk = talk;
        }

        void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
            // 会話履歴が更新されたタイミングでテキストを更新
            _talk.Log.ObserveAdd().Subscribe(_ => UpdateTalkHistory()).AddTo(this);
            // 送信ボタンを押したもしくはEnterキーを押したら入力を決定
            this.UpdateAsObservable()
                .Where(_ => _isOpened)
                .Where(_ => Input.GetKeyDown(KeyCode.Return))
                .Subscribe(_ => SubmitInput());
            _enterButton.onClick.AddListener(SubmitInput);
        }

        void Start()
        {
            StateChange(_closedWidth, 0, _closedLetter);
            _switchButton.onClick.AddListener(Switch);
            _logText.text = "";
        }

        // チャット画面の開閉
        void Switch()
        {
            if (_isOpened) StateChange(_closedWidth, _animationSpeed, _closedLetter);
            else StateChange(_OpenedWidth, _animationSpeed, _opendLetter);

            AudioPlayer.Play(AudioKey.TabOpenCloseSE, AudioPlayer.PlayMode.SE);
            _isOpened = !_isOpened;
        }

        // チャット画面の状態を切り替える
        void StateChange(float to, float duration, string letter)
        {
            WindowAnimationAsync(to, duration, _token).Forget();
            _switchButton.GetComponentInChildren<Text>().text = letter;
        }

        // チャット画面開閉のアニメーション
        async UniTaskVoid WindowAnimationAsync(float to, float duration, CancellationToken token)
        {
            Vector3 a = _window.localPosition;
            Vector3 b = new Vector3(to, a.y, a.z);

            // アニメーション中はボタンを押せない
            _switchButton.interactable = false;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _window.localPosition = Vector3.Lerp(a, b, t / duration);
                await UniTask.Yield();
            }
            _window.localPosition = b;
            _switchButton.interactable = true;
        }

        // 会話履歴を更新
        void UpdateTalkHistory()
        {
            _builder.Clear();
            foreach (string s in _talk.Log)
            {
                _builder.AppendLine(s);
            }

            _logText.text = _builder.ToString();
        }

        // 入力を決定
        void SubmitInput()
        {
            AudioPlayer.Play(AudioKey.PlayerSendSE, AudioPlayer.PlayMode.SE);

            if (_inputField.text == "") return;

            _talk.GameRuleAI.AddOption(_inputField.text, _talk.Settings.PlayerPriority, Talk.Owner.Player);
            _talk.CharacterAI.AddOption(_inputField.text, _talk.Settings.PlayerPriority, Talk.Owner.Player);
            _talk.AddLog(_logHeader, _inputField.text);
            _inputField.text = "";
        }
    }
}