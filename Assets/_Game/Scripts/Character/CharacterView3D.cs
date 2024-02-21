using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Text;
using VContainer;
using UniRx;
using TMPro;

namespace PSB.Game
{
    public class CharacterView3D : MonoBehaviour
    {
        [Header("�L�����N�^�[��G��")]
        [SerializeField] Button _touchArea;
        [SerializeField] TextAsset _touchedLines;
        [Header("�ؑփ{�^���̕�����ς���")]
        [SerializeField] string _fullModeLetter = "�V���v��";
        [SerializeField] string _simpleModeLetter = "�t��";
        [Header("�e�L�X�g�̍X�V")]
        [SerializeField] Text _fullModeText;
        [SerializeField] float _textFeed = 0.05f;
        [Header("�S��")]
        [SerializeField] Transform _mentalGauge;

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
            _talk.CharacterAI.Response.Skip(1).Subscribe(AiResponse).AddTo(this);
            _talk.Mental.Skip(1).Subscribe(MentalGaugeValueChanged).AddTo(this);
            MentalGaugeValueChanged(_talk.Mental.Value);
            // �L�����N�^�[��G�����璝��
            LoadPreparedLines();
            _touchArea.onClick.AddListener(PreparedLine);
        }

        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
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

            AudioPlayer.Play(AudioKey.TabOpenCloseSE, AudioPlayer.PlayMode.SE);
        }

        // ���炩���ߗp�ӂ��ꂽ�����_���ȑ䎌�𒝂�
        void PreparedLine()
        {
            AudioPlayer.Play(AudioKey.CharacterTouchSE, AudioPlayer.PlayMode.SE);
            
            string s = _preparedLines[Random.Range(0, _preparedLines.Length)];
            Print(s);
        }

        // AI����̃��X�|���X��\��
        void AiResponse(string line)
        {
            Print(line);
            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // �e�L�X�g�ɕ\��
        void Print(string line)
        {
            _cts?.Cancel();
            _cts = new();
            TextAnimationAsync(line, _cts.Token).Forget();
        }

        // ��������A�j���[�V����
        async UniTaskVoid TextAnimationAsync(string line, CancellationToken token)
        {
            _builder.Clear();

            if (line == null) return;

            for (int i = 0; i < line.Length; i++)
            {
                if (token.IsCancellationRequested) break;

                _builder.Append(line[i]);
                _fullModeText.text = _builder.ToString();

                await UniTask.WaitForSeconds(_textFeed, cancellationToken: token);
            }
        }

        // �S��̃Q�[�W��ύX
        void MentalGaugeValueChanged(int value)
        {
            // �傫����01�ŕύX
            float v = value * 1.0f / _talk.Settings.MaxMental;
            _mentalGauge.transform.localScale = new Vector3(v, 1, 1);
        }
    }
}
