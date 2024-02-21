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
        [Header("�L�����N�^�[�Ƃ̉�b")]
        [SerializeField] InputField _inputField;
        [SerializeField] Button _enterButton;
        [SerializeField] Text _logText;
        [SerializeField] string _logHeader = "���Ȃ�: ";
        [Header("�`���b�g��ʂ̊J��")]
        [SerializeField] Button _switchButton;
        [SerializeField] Transform _window;
        [SerializeField] float _closedWidth = -429.0f;
        [SerializeField] float _OpenedWidth = 19.0f;
        [SerializeField] float _animationSpeed = 1.0f;
        [Header("�J������{�^���̕�����ς���")]
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
            // ��b�������X�V���ꂽ�^�C�~���O�Ńe�L�X�g���X�V
            _talk.Log.ObserveAdd().Subscribe(_ => UpdateTalkHistory()).AddTo(this);
            // ���M�{�^������������������Enter�L�[������������͂�����
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

        // �`���b�g��ʂ̊J��
        void Switch()
        {
            if (_isOpened) StateChange(_closedWidth, _animationSpeed, _closedLetter);
            else StateChange(_OpenedWidth, _animationSpeed, _opendLetter);

            AudioPlayer.Play(AudioKey.TabOpenCloseSE, AudioPlayer.PlayMode.SE);
            _isOpened = !_isOpened;
        }

        // �`���b�g��ʂ̏�Ԃ�؂�ւ���
        void StateChange(float to, float duration, string letter)
        {
            WindowAnimationAsync(to, duration, _token).Forget();
            _switchButton.GetComponentInChildren<Text>().text = letter;
        }

        // �`���b�g��ʊJ�̃A�j���[�V����
        async UniTaskVoid WindowAnimationAsync(float to, float duration, CancellationToken token)
        {
            Vector3 a = _window.localPosition;
            Vector3 b = new Vector3(to, a.y, a.z);

            // �A�j���[�V�������̓{�^���������Ȃ�
            _switchButton.interactable = false;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _window.localPosition = Vector3.Lerp(a, b, t / duration);
                await UniTask.Yield();
            }
            _window.localPosition = b;
            _switchButton.interactable = true;
        }

        // ��b�������X�V
        void UpdateTalkHistory()
        {
            _builder.Clear();
            foreach (string s in _talk.Log)
            {
                _builder.AppendLine(s);
            }

            _logText.text = _builder.ToString();
        }

        // ���͂�����
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