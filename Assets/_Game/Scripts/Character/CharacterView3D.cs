using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Text;
using VContainer;
using UniRx;

namespace PSB.Game
{
    public class CharacterView3D : MonoBehaviour
    {
        [Header("�L�����N�^�[��G��")]
        [SerializeField] Button _touchArea;
        [SerializeField] TextAsset _touchedLines;
        [Header("�\�����[�h�̐؂�ւ�")]
        [SerializeField] CanvasGroup _full;
        [SerializeField] GameObject _character3D;
        [SerializeField] CanvasGroup _simple;
        [SerializeField] Button _changeButton;
        [Header("�ؑփ{�^���̕�����ς���")]
        [SerializeField] string _fullModeLetter = "�V���v��";
        [SerializeField] string _simpleModeLetter = "�t��";
        [Header("�e�L�X�g�̍X�V")]
        [SerializeField] Text _fullModeText;
        [SerializeField] Text _simpleModeText;
        [SerializeField] float _textFeed = 0.05f;

        TalkState _talkState;
        CancellationTokenSource _cts;
        StringBuilder _builder = new();
        string[] _preparedLines;
        bool _isFull = true;

        [Inject]
        void Construct(TalkState talkState)
        {
            _talkState = talkState;
        }

        void Awake()
        {
            _fullModeText.text = "";
            _simpleModeText.text = "";
            _talkState.CharacterLine.Skip(1).Subscribe(Print);
            _changeButton.onClick.AddListener(Switch);
            StateChange(_isFull);
            // �L�����N�^�[��G�����璝��
            LoadPreparedLines();
            _touchArea.onClick.AddListener(PreparedLine);
        }

        void OnDestroy()
        {
            if (_cts != null) _cts.Cancel();
        }

        // �^�b�`���ꂽ�ۂɒ���䎌��ǂݍ���
        void LoadPreparedLines()
        {
            _preparedLines = _touchedLines.ToString().Split("\n");
        }

        // �V���v���ƃt����؂�ւ���
        void Switch()
        {
            _isFull = !_isFull;
            StateChange(_isFull);

            AudioPlayer.Play(AudioKey.TabOpenCloseSE, AudioPlayer.PlayMode.SE);
        }

        // CanvasGroup�̃A���t�@�l��M�違�L�����N�^�[��\����\����؂�ւ���
        void StateChange(bool isFull)
        {
            _full.alpha = isFull ? 1 : 0;
            _simple.alpha = isFull ? 0 : 1;
            _character3D.SetActive(isFull);

            Text t = _changeButton.GetComponentInChildren<Text>();
            t.text = isFull ? _fullModeLetter : _simpleModeLetter;
        }

        // ���炩���ߗp�ӂ��ꂽ�����_���ȑ䎌�𒝂�
        void PreparedLine()
        {
            AudioPlayer.Play(AudioKey.CharacterTouchSE, AudioPlayer.PlayMode.SE);

            string s = _preparedLines[Random.Range(0, _preparedLines.Length)];
            Print(s);
        }

        // �e�L�X�g�ɕ\��
        void Print(string line)
        {
            if (_cts != null) _cts.Cancel();
            _cts = new();
            TextAnimationAsync(line, _cts.Token).Forget();

            // NOTE:�����ŃA�j���[�V�����̍Đ��Ƃ�����
        }

        // ��������A�j���[�V����
        async UniTaskVoid TextAnimationAsync(string line, CancellationToken token)
        {
            _builder.Clear();

            if (line == null) return;

            for (int i = 0; i < line.Length; i++)
            {
                _builder.Append(line[i]);
                string s = _builder.ToString();
                _simpleModeText.text = s;
                _fullModeText.text = s;
                await UniTask.WaitForSeconds(_textFeed, cancellationToken: token);
            }
        }
    }
}
