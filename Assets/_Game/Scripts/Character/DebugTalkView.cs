using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using UniRx;

namespace PSB.Game
{
    public class DebugTalkView : MonoBehaviour
    {
        [SerializeField] Text _aiRequestView;
        [SerializeField] Text _gameRuleAiResponseView;
        [SerializeField] Text _characterAiResponseView;
        [SerializeField] Text _mental;

        Talk _talk;

        [Inject]
        void Construct(Talk talk)
        {
            _talk = talk;
        }

        void Start()
        {
            _talk.AiRequest.Skip(1).Subscribe(s => 
            {
                _aiRequestView.text = $"リクエスト: {s}";
            }).AddTo(this);

            _talk.GameRuleAiResponse.Skip(1).Subscribe(s =>
            {
                _gameRuleAiResponseView.text = $"ゲームルールからのレスポンス: {s}";
            }).AddTo(this);

            _talk.CharacterAiResponse.Skip(1).Subscribe(s => 
            {
                _characterAiResponseView.text = $"キャラクターからのレスポンス: {s}";
            }).AddTo(this);
        }
    }
}
