using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

namespace PSB.Game
{
    /// <summary>
    /// Character�̃N���X�ԂŒl������肷��
    /// </summary>
    public class Talk
    {
        /// <summary>
        /// �N�����M�������b�Z�[�W���𔻒肷��
        /// </summary>
        public enum Owner { Player, GameState}

        /// <summary>
        /// AI�Ƀ��N�G�X�g���镶�͂Ɋւ�����
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
            /// ���͖{��
            /// </summary>
            public string Text { get; private set; }
            /// <summary>
            /// �D��x
            /// </summary>
            public int Priority { get; private set; }
            /// <summary>
            /// �N�����M������
            /// </summary>
            public Owner Owner { get; private set; }

            public int CompareTo(Message other)
            {
                return Priority - other.Priority;
            }
        }

        /// <summary>
        /// AI�Ƃ̂����Ɋւ���l����ނ��Ƃɂ܂Ƃ߂ĊǗ�����
        /// </summary>
        public class Content
        {
            ReactiveProperty<string> _request = new();
            ReactiveProperty<string> _response = new();
            // ���N�G�X�g�̌��ɒǉ�����O�ɕ��͂ɔC�ӂ̕����ŏC�����鎖���o����B
            Func<string, string> _requestPrefix;
            // �v���C���[�����M�������̓Q�[���̏�Ԃ�ϊ��������N�G�X�g�̌��B
            List<Message> _options = new();

            public Content(Func<string, string> requestPrefix = null)
            {
                _requestPrefix = requestPrefix;
            }

            /// <summary>
            /// AI�Ƀ��N�G�X�g��������
            /// </summary>
            public IReadOnlyReactiveProperty<string> Request => _request;
            /// <summary>
            /// AI����̃��X�|���X�̕���
            /// </summary>
            public IReadOnlyReactiveProperty<string> Response => _response;

            /// <summary>
            /// ���N�G�X�g���̒ǉ�
            /// </summary>
            public void AddOption(string text, int priority, Owner owner)
            {
                if (_requestPrefix != null) text = _requestPrefix(text);
                _options.Add(new(text, priority, owner));
            }

            /// <summary>
            /// ��ԗD��x���������N�G�X�g���擾���A���b�Z�[�W�̌���S�č폜����B
            /// </summary>
            public Message SelectTopPriorityOption()
            {
                Message m = _options.OrderByDescending(m => m).FirstOrDefault();
                _options.Clear();

                _request.Value = m != null ? m.Text : "";
                return m;
            }

            /// <summary>
            /// AI����̃��X�|���X���Z�b�g
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
        /// �L�����N�^�[���̐ݒ�
        /// </summary>
        public CharacterSettings Settings => _settings;
        /// <summary>
        /// �L�����N�^�[�̐S��
        /// </summary>
        public IReadOnlyReactiveProperty<int> Mental => _mental;
        /// <summary>
        /// �L�����N�^�[�̐S��̕ω���
        /// </summary>
        public IReadOnlyReactiveProperty<int> DeltaMental => _deltaMental;
        /// <summary>
        /// ��b����
        /// </summary>
        public IReadOnlyReactiveCollection<string> Log => _log;

        public Content CharacterAI => _characterAI;
        public Content GameRuleAI => _gameRuleAI;

        /// <summary>
        /// ��b�����ɑ䎌��ǉ�
        /// </summary>
        public void AddLog(string header, string text)
        {
            _log.Add(header + text);
        }

        /// <summary>
        /// �L�����N�^�[�̐S��̒l���Z�b�g
        /// </summary>
        public void SetMental(int value)
        {
            _deltaMental.Value = value - _mental.Value;
            _mental.Value = Mathf.Clamp(value, _settings.MinMental, _settings.MaxMental);
        }

        // ���N�G�X�g�̌��Ƃ��Ēǉ�����O�ɐS��ɂ���Č������ς��悤�ɏC���B
        string FixByMental(string text)
        {
            // �S��ő�l�̔����ȉ��̏ꍇ�͕s�@���ɂȂ�B
            if (_mental.Value < _settings.MaxMental / 2)
            {
                return "���̕��͂ɕs�@���ȑԓx�œ����Ă��������B" + text;
            }

            return text;
        }
    }
}
