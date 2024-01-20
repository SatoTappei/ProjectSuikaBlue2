using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Text;

namespace PSB.Game
{
    /// <summary>
    /// Character�̃N���X�ԂŒl������肷��
    /// </summary>
    public class TalkState
    {
        CharacterSettings _settings;
        StringBuilder _builder = new();
        Queue<string> _playerSends = new();
        ReactiveCollection<string> _log = new();
        ReactiveProperty<string> _characterLine = new();
        // ����f�o�b�O�ŕ\��������p
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
        /// �L�����N�^�[�̐S��
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
        /// �L�����N�^�[���̐ݒ�
        /// </summary>
        public CharacterSettings Settings => _settings;
        /// <summary>
        /// ��b����
        /// </summary>
        public IReadOnlyReactiveCollection<string> Log => _log;
        /// <summary>
        /// ��b�����ɑ䎌��ǉ������ۂɌĂ΂��
        /// </summary>
        public IObservable<CollectionAddEvent<string>> AddLogObservable => _log.ObserveAdd();
        /// <summary>
        /// �L�����N�^�[�̑䎌
        /// </summary>
        public IReadOnlyReactiveProperty<string> CharacterLine => _characterLine;
        /// <summary>
        /// ���Ɏ��o�����v���C���[�̓���
        /// </summary>
        public string LeadPlayerSend => _playerSends.TryPeek(out string r) ? r : "";
        /// <summary>
        /// OpenAI���v���C���[�̓��͂̕����𔻒肵�����X�|���X
        /// </summary>
        public IReadOnlyReactiveProperty<string> ContextJudge => _contextJudge;
        /// <summary>
        /// �f�o�b�O�p:�v���C���[�̎w���ɏ]���ꍇ�̃v���C���[��OpenAI�ւ̃��N�G�X�g
        /// </summary>
        public IReadOnlyReactiveProperty<string> PlayerFollowRequest => _playerFollowRequest;
        /// <summary>
        /// �f�o�b�O�p:�v���C���[�̎w���ɏ]���ꍇ��OpenAI����̃��X�|���X
        /// </summary>
        public IReadOnlyReactiveProperty<string> PlayerFollowResponse => _playerFollowResponse;
        /// <summary>
        /// �f�o�b�O�p:�Q�[���̏�Ԃ��画�f����ꍇ�̃v���C���[��OpenAI�ւ̃��N�G�X�g
        /// </summary>
        public IReadOnlyReactiveProperty<string> GameStateJudgeRequest => _gameStateJudgeRequest;
        /// <summary>
        /// �f�o�b�O�p:�Q�[���̏�Ԃ��画�f����ꍇ��OpenAI����̃��X�|���X
        /// </summary>
        public IReadOnlyReactiveProperty<string> GameStateJudgeResponse => _gameStateJudgeResponse;

        /// <summary>
        /// ��b�����ɑ䎌��ǉ�
        /// </summary>
        public void AddLog(string header, string line)
        {
            _builder.Clear();
            _builder.Append(header);
            _builder.Append(line);
            _log.Add(_builder.ToString());
        }

        /// <summary>
        /// �v���C���[�̓��͂�ǉ�
        /// </summary>
        public void AddPlayerSend(string line)
        {
            _playerSends.Enqueue(line);
        }

        /// <summary>
        /// �v���C���[�̓��͂���擪���擾���A�L���[���̂���ɂ���
        /// </summary>
        public string GetPlayerSend()
        {
            if (_playerSends.Count == 0) return "";

            string s = _playerSends.Dequeue();
            _playerSends.Clear();
            return s;
        }

        /// <summary>
        /// �L�����N�^�[�̑䎌���Z�b�g
        /// </summary>
        public void SetCharacterLine(string line)
        {
            _characterLine.Value = line;
        }

        /// <summary>
        /// OpenAI���v���C���[�̓��͂̕����𔻒肵�����X�|���X���Z�b�g
        /// </summary>
        public void SetContextJudgeResponse(string line)
        {
            _contextJudge.Value = line;
        }

        /// <summary>
        /// �v���C���[�̎w���ɏ]���ꍇ��OpenAI�Ƃ̂������Z�b�g
        /// </summary>
        public void SetPlayerFollowTalk(string request, string response)
        {
            _playerFollowRequest.Value = request;
            _playerFollowResponse.Value = response;
        }

        /// <summary>
        /// �Q�[���̏�Ԃ�API�����f���Ď��̍s�������߂�ꍇ��OpenAI�Ƃ̂������Z�b�g
        /// </summary>
        public void SetGameStateJudgeTalk(string request, string response)
        {
            _gameStateJudgeRequest.Value = request;
            _gameStateJudgeResponse.Value = response;
        }
    }
}
