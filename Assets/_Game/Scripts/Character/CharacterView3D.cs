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
        [Header("キャラクターを触る")]
        [SerializeField] Button _touchArea;
        [SerializeField] TextAsset _touchedLines;
        [Header("切替ボタンの文字を変える")]
        [SerializeField] string _fullModeLetter = "シンプル";
        [SerializeField] string _simpleModeLetter = "フル";
        [Header("テキストの更新")]
        [SerializeField] Text _fullModeText;
        [SerializeField] float _textFeed = 0.05f;
        [Header("心情")]
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
            // キャラクターを触ったら喋る
            LoadPreparedLines();
            _touchArea.onClick.AddListener(PreparedLine);
        }

        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        // タッチされた際に喋る台詞を読み込む
        void LoadPreparedLines()
        {
            _preparedLines = _touchedLines.ToString().Split("\n");
        }

        // シンプルとフルを切り替える
        void Switch()
        {
            _isFull = !_isFull;

            AudioPlayer.Play(AudioKey.TabOpenCloseSE, AudioPlayer.PlayMode.SE);
        }

        // あらかじめ用意されたランダムな台詞を喋る
        void PreparedLine()
        {
            AudioPlayer.Play(AudioKey.CharacterTouchSE, AudioPlayer.PlayMode.SE);
            
            string s = _preparedLines[Random.Range(0, _preparedLines.Length)];
            Print(s);
        }

        // AIからのレスポンスを表示
        void AiResponse(string line)
        {
            Print(line);
            AudioPlayer.Play(AudioKey.CharacterSendSE, AudioPlayer.PlayMode.SE);
        }

        // テキストに表示
        void Print(string line)
        {
            _cts?.Cancel();
            _cts = new();
            TextAnimationAsync(line, _cts.Token).Forget();
        }

        // 文字送りアニメーション
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

        // 心情のゲージを変更
        void MentalGaugeValueChanged(int value)
        {
            // 大きさを01で変更
            float v = value * 1.0f / _talk.Settings.MaxMental;
            _mentalGauge.transform.localScale = new Vector3(v, 1, 1);
        }
    }
}
