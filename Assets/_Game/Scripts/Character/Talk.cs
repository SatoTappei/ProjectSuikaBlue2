using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

namespace PSB.Game
{
    /// <summary>
    /// Characterのクラス間で値をやり取りする
    /// </summary>
    public class Talk
    {
        /// <summary>
        /// 誰が送信したメッセージかを判定する
        /// </summary>
        public enum Owner { Player, GameState}

        /// <summary>
        /// AIにリクエストする文章に関する情報
        /// </summary>
        public class Message : IComparable<Message>
        {
            public Message(string text, int priority, Owner owner)
            {
                Text = text;
                Priority = priority;
                Owner = owner;
            }

            /// <summary>
            /// 文章本体
            /// </summary>
            public string Text { get; private set; }
            /// <summary>
            /// 優先度
            /// </summary>
            public int Priority { get; private set; }
            /// <summary>
            /// 誰が送信したか
            /// </summary>
            public Owner Owner { get; private set; }

            public int CompareTo(Message other)
            {
                return Priority - other.Priority;
            }
        }

        /// <summary>
        /// AIとのやり取りに関する値を種類ごとにまとめて管理する
        /// </summary>
        public class Content
        {
            ReactiveProperty<string> _request = new();
            ReactiveProperty<string> _response = new();
            // リクエストの候補に追加する前に文章に任意の文字で修正する事が出来る。
            Func<string, string> _requestPrefix;
            // プレイヤーが送信もしくはゲームの状態を変換したリクエストの候補。
            List<Message> _options = new();

            public Content(Func<string, string> requestPrefix = null)
            {
                _requestPrefix = requestPrefix;
            }

            /// <summary>
            /// AIにリクエストした文章
            /// </summary>
            public IReadOnlyReactiveProperty<string> Request => _request;
            /// <summary>
            /// AIからのレスポンスの文章
            /// </summary>
            public IReadOnlyReactiveProperty<string> Response => _response;

            /// <summary>
            /// リクエスト候補の追加
            /// </summary>
            public void AddOption(string text, int priority, Owner owner)
            {
                if (_requestPrefix != null) text = _requestPrefix(text);
                _options.Add(new(text, priority, owner));
            }

            /// <summary>
            /// 一番優先度が高いリクエストを取得し、メッセージの候補を全て削除する。
            /// </summary>
            public Message SelectTopPriorityOption()
            {
                Message m = _options.OrderByDescending(m => m).FirstOrDefault();
                _options.Clear();

                _request.Value = m != null ? m.Text : "";
                return m;
            }

            /// <summary>
            /// AIからのレスポンスをセット
            /// </summary>
            public void SetResponse(string text)
            {
                _response.Value = text;
            }
        }

        readonly CharacterSettings _settings;
        readonly ReactiveProperty<int> _mental;
        readonly ReactiveProperty<int> _deltaMental = new();
        readonly ReactiveCollection<string> _log = new();
        readonly Content _characterAI;
        readonly Content _gameRuleAI;

        public Talk(CharacterSettings settings)
        {
            _settings = settings;
            _mental = new(settings.DefaultMental);
            _characterAI = new(FixByMental);
            _gameRuleAI = new();
        }

        /// <summary>
        /// キャラクター毎の設定
        /// </summary>
        public CharacterSettings Settings => _settings;
        /// <summary>
        /// キャラクターの心情
        /// </summary>
        public IReadOnlyReactiveProperty<int> Mental => _mental;
        /// <summary>
        /// キャラクターの心情の変化量
        /// </summary>
        public IReadOnlyReactiveProperty<int> DeltaMental => _deltaMental;
        /// <summary>
        /// 会話履歴
        /// </summary>
        public IReadOnlyReactiveCollection<string> Log => _log;

        public Content CharacterAI => _characterAI;
        public Content GameRuleAI => _gameRuleAI;

        /// <summary>
        /// 会話履歴に台詞を追加
        /// </summary>
        public void AddLog(string header, string text)
        {
            _log.Add(header + text);
        }

        /// <summary>
        /// キャラクターの心情の値をセット
        /// </summary>
        public void SetMental(int value)
        {
            _deltaMental.Value = value - _mental.Value;
            _mental.Value = Mathf.Clamp(value, _settings.MinMental, _settings.MaxMental);
        }

        // リクエストの候補として追加する前に心情によって口調が変わるように修正。
        string FixByMental(string text)
        {
            // 心情が最大値の半分以下の場合は不機嫌になる。
            if (_mental.Value < _settings.MaxMental / 2)
            {
                return "次の文章に不機嫌な態度で答えてください。" + text;
            }

            return text;
        }
    }
}
