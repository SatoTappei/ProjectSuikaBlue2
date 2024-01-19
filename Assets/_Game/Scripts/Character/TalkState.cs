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

        public TalkState(CharacterSettings settings)
        {
            _settings = settings;
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
        /// �v���C���[�����M����������
        /// </summary>
        public Queue<string> PlayerSends => _playerSends;
        /// <summary>
        /// �L�����N�^�[�̑䎌
        /// </summary>
        public IReadOnlyReactiveProperty<string> CharacterLine => _characterLine;

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
    }
}
