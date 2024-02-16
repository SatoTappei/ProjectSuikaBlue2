using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Text;
using System.Linq;

namespace PSB.Game
{
    /// <summary>
    /// Characterのクラス間で値をやり取りする
    /// </summary>
    public class Talk
    {
        /// <summary>
        /// AIにリクエストする文章を管理するためのクラス
        /// </summary>
        public class Message : IComparable<Message>
        {
            public Message(string text, int priority)
            {
                Text = text;
                Priority = priority;
            }

            /// <summary>
            /// リクエストする文章
            /// </summary>
            public string Text { get; private set; }
            /// <summary>
            /// 優先度
            /// </summary>
            public int Priority { get; private set; }

            public int CompareTo(Message other)
            {
                return Priority - other.Priority;
            }
        }

        CharacterSettings _settings;
        StringBuilder _builder = new();
        ReactiveCollection<string> _log = new();
        ReactiveProperty<string> _aiRequest = new();
        ReactiveProperty<string> _gameRuleAiResponse = new();
        ReactiveProperty<string> _characterAiResponse = new();
        // プレイヤーが送信した文章とゲームの状態を変換した文章どちらも一括で管理する。
        List<Message> _messages = new();
        int _mental;

        public Talk(CharacterSettings settings)
        {
            _settings = settings;
            _mental = settings.DefaultMental;
        }

        /// <summary>
        /// キャラクターの心情
        /// </summary>
        public int Mental
        {
            get => _mental;
            set
            {
                _mental = value;
                _mental = Mathf.Clamp(_mental, _settings.MinMental, _settings.MaxMental);
            }
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
        /// AIにリクエストする文章
        /// </summary>
        public IReadOnlyReactiveProperty<string> AiRequest => _aiRequest;
        /// <summary>
        /// ゲームルールAIからのレスポンス
        /// </summary>
        public IReadOnlyReactiveProperty<string> GameRuleAiResponse => _gameRuleAiResponse;
        /// <summary>
        /// キャラクターAIからのレスポンス
        /// </summary>
        public IReadOnlyReactiveProperty<string> CharacterAiResponse => _characterAiResponse;

        /// <summary>
        /// AIにリクエストする用のメッセージを追加
        /// </summary>
        public void AddMessage(string text, int priority)
        {
            _messages.Add(new(text, priority));
        }

        /// <summary>
        /// 優先度が最も高いメッセージを選択し、溜めたメッセージの中身を空にする。
        /// </summary>
        public Message SelectTopPriorityMessage()
        {
            Message m = _messages.OrderByDescending(m => m).FirstOrDefault();
            _messages.Clear();

            // 選択した文章を保持
            if (m != null) _aiRequest.Value = m.Text;

            return m;
        }

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
        /// ゲームルールAIからのレスポンスをセット
        /// </summary>
        public void SetGameRuleAiResponse(string line)
        {
            _gameRuleAiResponse.Value = line;
        }

        /// <summary>
        /// キャラクターAIからのレスポンスをセット
        /// </summary>
        public void SetCharacterAiResponse(string line)
        {
            _characterAiResponse.Value = line;
        }
    }
}
