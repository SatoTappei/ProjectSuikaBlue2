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
        // 現状デバッグで表示させる用
        ReactiveProperty<string> _contextJudge = new();
        ReactiveProperty<string> _playerFollowRequest = new();
        ReactiveProperty<string> _playerFollowResponse = new();
        ReactiveProperty<string> _gameStateJudgeRequest = new();
        ReactiveProperty<string> _gameStateJudgeResponse = new();
        int _mental;

        public TalkState(CharacterSettings settings)
        {
            _settings = settings;
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
        /// 会話履歴に台詞を追加した際に呼ばれる
        /// </summary>
        public IObservable<CollectionAddEvent<string>> AddLogObservable => _log.ObserveAdd();
        /// <summary>
        /// キャラクターの台詞
        /// </summary>
        public IReadOnlyReactiveProperty<string> CharacterLine => _characterLine;
        /// <summary>
        /// 次に取り出されるプレイヤーの入力
        /// </summary>
        public string LeadPlayerSend => _playerSends.TryPeek(out string r) ? r : "";
        /// <summary>
        /// OpenAIがプレイヤーの入力の文脈を判定したレスポンス
        /// </summary>
        public IReadOnlyReactiveProperty<string> ContextJudge => _contextJudge;
        /// <summary>
        /// デバッグ用:プレイヤーの指示に従う場合のプレイヤーのOpenAIへのリクエスト
        /// </summary>
        public IReadOnlyReactiveProperty<string> PlayerFollowRequest => _playerFollowRequest;
        /// <summary>
        /// デバッグ用:プレイヤーの指示に従う場合のOpenAIからのレスポンス
        /// </summary>
        public IReadOnlyReactiveProperty<string> PlayerFollowResponse => _playerFollowResponse;
        /// <summary>
        /// デバッグ用:ゲームの状態から判断する場合のプレイヤーのOpenAIへのリクエスト
        /// </summary>
        public IReadOnlyReactiveProperty<string> GameStateJudgeRequest => _gameStateJudgeRequest;
        /// <summary>
        /// デバッグ用:ゲームの状態から判断する場合のOpenAIからのレスポンス
        /// </summary>
        public IReadOnlyReactiveProperty<string> GameStateJudgeResponse => _gameStateJudgeResponse;

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

        /// <summary>
        /// OpenAIがプレイヤーの入力の文脈を判定したレスポンスをセット
        /// </summary>
        public void SetContextJudgeResponse(string line)
        {
            _contextJudge.Value = line;
        }

        /// <summary>
        /// プレイヤーの指示に従う場合のOpenAIとのやり取りをセット
        /// </summary>
        public void SetPlayerFollowTalk(string request, string response)
        {
            _playerFollowRequest.Value = request;
            _playerFollowResponse.Value = response;
        }

        /// <summary>
        /// ゲームの状態をAPIが判断して次の行動を決める場合のOpenAIとのやり取りをセット
        /// </summary>
        public void SetGameStateJudgeTalk(string request, string response)
        {
            _gameStateJudgeRequest.Value = request;
            _gameStateJudgeResponse.Value = response;
        }
    }
}
