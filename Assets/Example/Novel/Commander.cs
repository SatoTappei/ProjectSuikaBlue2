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
        /// �R�}���h�����s����C�x���g�ɕϊ�
        /// </summary>
        public async UniTask RunAsync(string[] commands, CancellationToken skipToken)
        {
            List<List<UniTask>> tasks = new List<List<UniTask>>();
            tasks.Add(new List<UniTask>()); // ����A����ł̃C�x���g���s�����Ή����Ă��Ȃ�

            foreach (string c in commands)
            {
                string[] split = c.Split();

                // �擪��+���t���Ă����ꍇ�̓V�[�P���V�����Ɏ��s�����
                if (split[0][0] == '+')
                {
                    split[0] = split[0].Substring(1);
                    // �V�[�P���V�����ɂ��閂�@
                }

                if (split[0] == "�L�����\��") tasks[0].Add(ShowCharacterAsync(split, skipToken));
                else if (split[0] == "�L��������") tasks[0].Add(HideCharacterAsync(split, skipToken));
                else if (split[0] == "�L�����グ") tasks[0].Add(CharacterMoveToFrontAsync(split, skipToken));
                else if (split[0] == "�L��������") tasks[0].Add(CharacterMoveToBackAsync(split, skipToken));
                else if (split[0] == "�L�����W�����v") tasks[0].Add(CharacterJumpAsync(split, skipToken));
                else Debug.LogWarning("�Ή�����C�x���g������: " + split[0]);
            }

            foreach (List<UniTask> parallel in tasks)
            {
                await parallel;
            }
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
