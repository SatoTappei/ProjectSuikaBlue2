using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Text;

namespace PSB.Game
{
    /// <summary>
    /// Characterのクラス間で値をやり取りする
    /// </summary>
    public class TalkState
    {
        CharacterSettings _settings;
        StringBuilder _builder = new();
        Queue<string> _playerSends = new();
        ReactiveCollection<string> _log = new();
        ReactiveProperty<string> _characterLine = new();

        public TalkState(CharacterSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// キャラクター毎の設定
        /// </summary>
        public CharacterSettings Settings => _settings;
        /// <summary>
        /// 会話履歴
        /// </summary>
        public IReadOnlyReactiveCollection<string> Log => _log;
        /// <summary>
        /// 会話履歴に台詞を追加した際に呼ばれる
        /// </summary>
        public IObservable<CollectionAddEvent<string>> AddLogObservable => _log.ObserveAdd();
        /// <summary>
        /// プレイヤーが送信した文字列
        /// </summary>
        public Queue<string> PlayerSends => _playerSends;
        /// <summary>
        /// キャラクターの台詞
        /// </summary>
        public IReadOnlyReactiveProperty<string> CharacterLine => _characterLine;

        /// <summary>
        /// 会話履歴に台詞を追加
        /// </summary>
        public void AddLog(string header, string line)
        {
            _builder.Clear();
            _builder.Append(header);
            _builder.Append(line);
            _log.Add(_builder.ToString());
        }

        /// <summary>
        /// プレイヤーの入力を追加
        /// </summary>
        public void AddPlayerSend(string line)
        {
            _playerSends.Enqueue(line);
        }

        /// <summary>
        /// プレイヤーの入力から先頭を取得し、キュー自体を空にする
        /// </summary>
        public string GetPlayerSend()
        {
            if (_playerSends.Count == 0) return "";

            string s = _playerSends.Dequeue();
            _playerSends.Clear();
            return s;
        }

        /// <summary>
        /// キャラクターの台詞をセット
        /// </summary>
        public void SetCharacterLine(string line)
        {
            _characterLine.Value = line;
        }
    }
}
