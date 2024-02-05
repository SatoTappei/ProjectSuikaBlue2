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
    public class CharacterView : MonoBehaviour
    {
        [Header("�L�����N�^�[��G��")]
        [SerializeField] Button _touchArea;
        [SerializeField] TextAsset _touchedLines;
        [Header("�\�����[�h�̐؂�ւ�")]
        [SerializeField] CanvasGroup _full;
        [SerializeField] CanvasGroup _simple;
        [SerializeField] Button _changeButton;
        [Header("�ؑփ{�^���̕�����ς���")]
        [SerializeField] string _fullModeLetter = "�V���v��";
        [SerializeField] string _simpleModeLetter = "�t��";
        [Header("�e�L�X�g�̍X�V")]
        [SerializeField] Text _fullModeText;
        [SerializeField] Text _simpleModeText;
        [SerializeField] float _textFeed = 0.05f;
        [Header("�L�����N�^�[�̕\���ς���")]
        [SerializeField] GameObject _sprite1;
        [SerializeField] GameObject _sprite2;

        Talk _talk;
        CancellationTokenSource _cts;
        StringBuilder _builder = new();
        string[] _preparedLines;
        bool _isFull = true;

        [Inject]
        void Construct(Talk talk)
        {
            _talk = talk;
        }

        void Awake()
        {
            _fullModeText.text = "";
            _simpleModeText.text = "";
            _talk.CharacterLine.Skip(1).Subscribe(Print);
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

        // CanvasGroup�̃A���t�@�l��M��
        void StateChange(bool isFull)
        {
            _full.alpha = isFull ? 1 : 0;
            _simple.alpha = isFull ? 0 : 1;

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

            // �����_���ŕ\���ς���
            if (Random.value <= 0.5) { _sprite1.SetActive(true); _sprite2.SetActive(false); }
            else { _sprite1.SetActive(false); _sprite2.SetActive(true); }
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