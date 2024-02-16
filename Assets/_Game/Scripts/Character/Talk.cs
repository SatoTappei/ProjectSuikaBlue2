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
    /// Character�̃N���X�ԂŒl������肷��
    /// </summary>
    public class Talk
    {
        /// <summary>
        /// AI�Ƀ��N�G�X�g���镶�͂��Ǘ����邽�߂̃N���X
        /// </summary>
        public class Message : IComparable<Message>
        {
            public Message(string text, int priority)
            {
                Text = text;
                Priority = priority;
            }

            /// <summary>
            /// ���N�G�X�g���镶��
            /// </summary>
            public string Text { get; private set; }
            /// <summary>
            /// �D��x
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
        // �v���C���[�����M�������͂ƃQ�[���̏�Ԃ�ϊ��������͂ǂ�����ꊇ�ŊǗ�����B
        List<Message> _messages = new();
        int _mental;

        public Talk(CharacterSettings settings)
        {
            _settings = settings;
            _mental = settings.DefaultMental;
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
        /// AI�Ƀ��N�G�X�g���镶��
        /// </summary>
        public IReadOnlyReactiveProperty<string> AiRequest => _aiRequest;
        /// <summary>
        /// �Q�[�����[��AI����̃��X�|���X
        /// </summary>
        public IReadOnlyReactiveProperty<string> GameRuleAiResponse => _gameRuleAiResponse;
        /// <summary>
        /// �L�����N�^�[AI����̃��X�|���X
        /// </summary>
        public IReadOnlyReactiveProperty<string> CharacterAiResponse => _characterAiResponse;

        /// <summary>
        /// AI�Ƀ��N�G�X�g����p�̃��b�Z�[�W��ǉ�
        /// </summary>
        public void AddMessage(string text, int priority)
        {
            _messages.Add(new(text, priority));
        }

        /// <summary>
        /// �D��x���ł��������b�Z�[�W��I�����A���߂����b�Z�[�W�̒��g����ɂ���B
        /// </summary>
        public Message SelectTopPriorityMessage()
        {
            Message m = _messages.OrderByDescending(m => m).FirstOrDefault();
            _messages.Clear();

            // �I���������͂�ێ�
            if (m != null) _aiRequest.Value = m.Text;

            return m;
        }

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
        /// �Q�[�����[��AI����̃��X�|���X���Z�b�g
        /// </summary>
        public void SetGameRuleAiResponse(string line)
        {
            _gameRuleAiResponse.Value = line;
        }

        /// <summary>
        /// �L�����N�^�[AI����̃��X�|���X���Z�b�g
        /// </summary>
        public void SetCharacterAiResponse(string line)
        {
            _characterAiResponse.Value = line;
        }
    }
}
