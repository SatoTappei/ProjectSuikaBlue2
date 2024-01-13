using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Novel
{
    public class Commander : MonoBehaviour
    {
        [SerializeField] Character _centerCharacter;
        [SerializeField] Character _leftCharacter;
        [SerializeField] Character _rightCharacter;

        /// <summary>
        /// �V�[���J�n�O�������͏I�����ɃR�}���h�ŃC�x���g����eUI��������
        /// </summary>
        public void ResetAll()
        {
            _centerCharacter.Init();
            _leftCharacter.Init();
            _rightCharacter.Init();
        }

        /// <summary>
        /// �R�}���h�����s����C�x���g�ɕϊ�
        /// </summary>
        public async UniTask RunAsync(string[] commands, CancellationToken skipToken)
        {
            List<Queue<UniTask>> tasks = new();
            for (int i = 0; i < commands.Length; i++)
            {
                string[] split = commands[i].Split();

                // �O�̃C�x���g�ɑ����Ď��s����邩�`�F�b�N
                bool isAppend = split[0][0] == '+';
                if (isAppend) split[0] = split[0].Substring(1);

                // �擪�̃R�}���h�������͕���Ŏ��s�����C�x���g�̏ꍇ�͐V���ɃL���[��ǉ�
                if (i == 0 || !isAppend) tasks.Add(new());

                // �����Ď��s�����ꍇ�͑O�̃C�x���g�̃L���[�ɁA
                // ����Ŏ��s����ꍇ�͐V���ɒǉ����ꂽ�L���[�ɃC�x���g��ǉ�
                AddEvent(tasks[^1], split, skipToken);
            }

            // ����Ŏ��s�����L���[����1�����o���Ď��s���J��Ԃ�
            while (true)
            {
                // �S�ẴL���[���󂩂��`�F�b�N
                bool isEmpty = true;
                List<UniTask> temp = new();
                foreach (Queue<UniTask> q in tasks)
                {
                    if (q.Count == 0) continue;
                    
                    temp.Add(q.Dequeue());
                    isEmpty = false;
                }

                if (isEmpty) break;

                // ����Ŏ��s�����C�x���g���S�ďI���܂ő҂�
                await temp;
            }
        }

        // �R�}���h�ɑΉ������C�x���g���L���[�ɒǉ�
        void AddEvent(Queue<UniTask> q, string[] command, CancellationToken skipToken)
        {
            if (command[0] == "�L�����\��") q.Enqueue(ShowCharacterAsync(command, skipToken));
            else if (command[0] == "�L��������") q.Enqueue(HideCharacterAsync(command, skipToken));
            else if (command[0] == "�L�����グ") q.Enqueue(CharacterMoveToFrontAsync(command, skipToken));
            else if (command[0] == "�L��������") q.Enqueue(CharacterMoveToBackAsync(command, skipToken));
            else if (command[0] == "�L�����W�����v") q.Enqueue(CharacterJumpAsync(command, skipToken));
            else Debug.LogWarning("�Ή�����C�x���g������: " + command[0]);
        }

        // �L�����\��
        UniTask ShowCharacterAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            string character = split[2];
            float duration = float.Parse(split[3]);
            return UniTask.Defer(
                () => CharacterFrame(position).FadeInAsync(SelectCharacter(character), duration, skipToken));
        }

        // �L��������
        UniTask HideCharacterAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            float duration = float.Parse(split[2]);
            return UniTask.Defer(
                () => CharacterFrame(position).FadeOutAsync(duration, skipToken));
        }

        // �L�����グ
        UniTask CharacterMoveToFrontAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            // �K�v�ɉ����Ď��ԂɑΉ�������
            return UniTask.Defer(() 
                => CharacterFrame(position).SetOnFrontColorAsync(skipToken));
        }

        // �L��������
        UniTask CharacterMoveToBackAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            // �K�v�ɉ����Ď��ԂɑΉ�������
            return UniTask.Defer(
                () => CharacterFrame(position).SetOnBackColorAsync(skipToken));
        }

        // �L�����W�����v
        UniTask CharacterJumpAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            float duration = float.Parse(split[2]);
            int count = int.Parse(split[3]);
            return UniTask.Defer(
                () => CharacterFrame(position).JumpAsync(duration, count, skipToken));
        }

        Character CharacterFrame(string key)
        {
            if (key == "��") return _leftCharacter;
            else if (key == "����") return _centerCharacter;
            else if (key == "�E") return _rightCharacter;
            else throw new System.ArgumentException("�Ή�����t���[��������: " + key);
        }

        CharacterType SelectCharacter(string key)
        {
            if (key == "������") return CharacterType.Boy;
            else if (key == "��������") return CharacterType.Girl;
            else if (key == "����") return CharacterType.Oink;
            else throw new System.ArgumentException("�Ή�����L�����N�^�[������: " + key);
        }
    }
}
