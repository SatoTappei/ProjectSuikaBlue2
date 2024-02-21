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
        [SerializeField] Text _gameRuleAiRequest;
        [SerializeField] Text _characterAiRequest;
        [SerializeField] Text _gameRuleAiResponse;
        [SerializeField] Text _characterAiResponse;
        [SerializeField] Text _deltaMental;

        Talk _talk;

        [Inject]
        void Construct(Talk talk)
        {
            _talk = talk;
        }

        void Start()
        {
            _talk.GameRuleAI.Request.Subscribe(s => 
            {
                _gameRuleAiRequest.text = $"GameRuleRequest: {s}";
            }).AddTo(this);

            _talk.CharacterAI.Request.Subscribe(s =>
            {
                _characterAiRequest.text = $"CharacterRequest: {s}";
            }).AddTo(this);

            _talk.GameRuleAI.Response.Subscribe(s => 
            {
                _gameRuleAiResponse.text = $"GameRuleResponse: {s}";
            }).AddTo(this);

            _talk.CharacterAI.Response.Subscribe(s =>
            {
                _characterAiResponse.text = $"CharacterResponse: {s}";
            }).AddTo(this);

            _talk.DeltaMental.Subscribe(s =>
            {
                _deltaMental.text = $"DeltaMental: {s}";
            }).AddTo(this);
        }
    }
}
