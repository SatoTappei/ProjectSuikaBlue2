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
        [SerializeField] Text _contextJudge;
        [SerializeField] Text _playerFollowRequest;
        [SerializeField] Text _playerFollowResponse;
        [SerializeField] Text _gameStateJudgeRequest;
        [SerializeField] Text _gameStateJudgeResponse;
        [SerializeField] Text _mental;

        Talk _talk;

        [Inject]
        void Construct(Talk talk)
        {
            _talk = talk;
        }

        void Start()
        {
            _talk.ContextJudge.Skip(1).Subscribe(s => 
            {
                ContextJudgePrint(s);
            }).AddTo(this);

            _talk.PlayerFollowRequest.Skip(1).Subscribe(s =>
            {
                PlayerFollowRequestPrint(s);
                GameStateJudgeRequestPrint("");
            }).AddTo(this);

            _talk.PlayerFollowResponse.Skip(1).Subscribe(s => 
            {
                PlayerFollowResponsePrint(s);
                GameStateJudgeResponsePrint("");
                ContextJudgePrint("");
            }).AddTo(this);
            _talk.GameStateJudgeRequest.Skip(1).Subscribe(s => 
            {
                GameStateJudgeRequestPrint(s);
                PlayerFollowRequestPrint("");
            }).AddTo(this);
            _talk.GameStateJudgeResponse.Skip(1).Subscribe(s => 
            {
                GameStateJudgeResponsePrint(s);
                PlayerFollowResponsePrint("");
                ContextJudgePrint("");
            }).AddTo(this);
        }

        void ContextJudgePrint(string s) => _contextJudge.text = $"��������: {s}";
        void GameStateJudgeRequestPrint(string s)=> _gameStateJudgeRequest.text = $"�Q�[���̏�ԏ]�����N�G�X�g: {s}";
        void GameStateJudgeResponsePrint(string s) => _gameStateJudgeResponse.text = $"�Q�[���̏�ԏ]�����X�|���X: {s}";
        void PlayerFollowRequestPrint(string s) => _playerFollowRequest.text = $"�v���C���[�̎w�����N�G�X�g: {s}";
        void PlayerFollowResponsePrint(string s) => _playerFollowResponse.text = $"�v���C���[�̎w�����X�|���X: {s}";
    }
}
