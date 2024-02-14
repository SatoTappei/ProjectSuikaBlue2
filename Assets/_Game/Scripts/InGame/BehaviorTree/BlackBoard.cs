using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// �r�w�C�r�A�c���[���l��ǂݏ������邽�߂̍���
    /// 1�̃C���X�^���X�����L���邱�ƂŃL�����N�^�[���m�̘A�g��z�肵�Ă���B
    /// </summary>
    public class BlackBoard
    {
        int _id;

        Dictionary<int, Private> _all = new();

        /// <summary>
        /// �L�����N�^�[���̍��𐶐��B
        /// ������������ID�ŊǗ������̂ŁA�O������ID�ł̎擾���\�B
        /// </summary>
        public Private CreatePrivate()
        {
            int id = _id++;
            Private p = new();
            _all.Add(id, p);

            return p;
        }

        /// <summary>
        /// ID�őΉ����������擾�B
        /// </summary>
        public Private GetPrivate(int id)
        {
            if (_all.ContainsKey(id)) return _all[id];
            else throw new KeyNotFoundException("ID�ɑΉ������L�����N�^�[���̍�������: " + id);
        }

        // �ʂ̒l���������ޗp�̃N���X
        public class Private
        {
            public IReadOnlyList<IReadOnlyCell> Path { get; set; }
        }
    }
}