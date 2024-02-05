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
        [Header("キャラクターを触る")]
        [SerializeField] Button _touchArea;
        [SerializeField] TextAsset _touchedLines;
        [Header("表示モードの切り替え")]
        [SerializeField] CanvasGroup _full;
        [SerializeField] CanvasGroup _simple;
        [SerializeField] Button _changeButton;
        [Header("切替ボタンの文字を変える")]
        [SerializeField] string _fullModeLetter = "シンプル";
        [SerializeField] string _simpleModeLetter = "フル";
        [Header("テキストの更新")]
        [SerializeField] Text _fullModeText;
        [SerializeField] Text _simpleModeText;
        [SerializeField] float _textFeed = 0.05f;
        [Header("キャラクターの表情を変える")]
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
            StateChange(_isFull);

            AudioPlayer.Play(AudioKey.TabOpenCloseSE, AudioPlayer.PlayMode.SE);
        }

        // CanvasGroupのアルファ値を弄る
        void StateChange(bool isFull)
        {
            _full.alpha = isFull ? 1 : 0;
            _simple.alpha = isFull ? 0 : 1;

            Text t = _changeButton.GetComponentInChildren<Text>();
            t.text = isFull ? _fullModeLetter : _simpleModeLetter;
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

            // ランダムで表情を変える
            if (Random.value <= 0.5) { _sprite1.SetActive(true); _sprite2.SetActive(false); }
            else { _sprite1.SetActive(false); _sprite2.SetActive(true); }
        }

        // 文字送りアニメーション
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