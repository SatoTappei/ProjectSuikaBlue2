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
        [Header("キャラクターを触る")]
        [SerializeField] Button _touchArea;
        [SerializeField] TextAsset _touchedLines;
        [Header("切替ボタンの文字を変える")]
        [SerializeField] string _fullModeLetter = "シンプル";
        [SerializeField] string _simpleModeLetter = "フル";
        [Header("テキストの更新")]
        [SerializeField] Text _fullModeText;
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
            _talkState.CharacterLine.Skip(1).Subscribe(Print);
            // キャラクターを触ったら喋る
            LoadPreparedLines();
            _touchArea.onClick.AddListener(PreparedLine);
        }

        void OnDestroy()
        {
            if (_cts != null) _cts.Cancel();
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

        // テキストに表示
        void Print(string line)
        {
            if (_cts != null) _cts.Cancel();
            _cts = new();
            TextAnimationAsync(line, _cts.Token).Forget();

            // NOTE:ここでアニメーションの再生とかする
        }

        // 文字送りアニメーション
        async UniTaskVoid TextAnimationAsync(string line, CancellationToken token)
        {
            _builder.Clear();

            if (line == null) return;

            for (int i = 0; i < line.Length; i++)
            {
                _builder.Append(line[i]);
                _fullModeText.text = _builder.ToString();
                await UniTask.WaitForSeconds(_textFeed, cancellationToken: token);
            }
        }
    }
}
