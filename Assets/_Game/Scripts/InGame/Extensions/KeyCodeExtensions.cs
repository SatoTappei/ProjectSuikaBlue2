using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class KeyCodeExtensions
    {
        /// <summary>
        /// �w�肵�������������Ă����Ԃł̓��͎��ɑO��ړ�������P�ʕ����x�N�g��
        /// </summary>
        public static Vector3 ToDirectionVector(this KeyCode key, Direction current)
        {
            if (key == KeyCode.W) return current.ToVector3();
            if (key == KeyCode.S) return -current.ToVector3();

            throw new System.ArgumentException("�ړ��̓��͂��s���L�[�ł͂Ȃ�: " + key);
        }

        /// <summary>
        /// ���݌����Ă������������͂����L�[�ɉ�������]���s������̕���
        /// </summary>
        public static Direction ToTurnedDirection(this KeyCode key, Direction current)
        {
            // W�L�[�̏ꍇ�͂��̕����Ɉړ�����̂Ō����͕ς��Ȃ�
            // S�L�[�̏ꍇ�͔��Ε����Ɍ���
            // A�L�[�̏ꍇ�͔����v����90�x��]����
            // D�L�[�̏ꍇ�͎��v����90�x��]����

            switch (current)
            {
                case Direction.East when key == KeyCode.A: return Direction.North;
                case Direction.East when key == KeyCode.S: return Direction.West;
                case Direction.East when key == KeyCode.D: return Direction.South;
                case Direction.East when key == KeyCode.W: return Direction.East;

                case Direction.West when key == KeyCode.A: return Direction.South;
                case Direction.West when key == KeyCode.S: return Direction.East;
                case Direction.West when key == KeyCode.D: return Direction.North;
                case Direction.West when key == KeyCode.W: return Direction.West;

                case Direction.South when key == KeyCode.A: return Direction.East;
                case Direction.South when key == KeyCode.S: return Direction.North;
                case Direction.South when key == KeyCode.D: return Direction.West;
                case Direction.South when key == KeyCode.W: return Direction.South;

                case Direction.North when key == KeyCode.A: return Direction.West;
                case Direction.North when key == KeyCode.S: return Direction.South;
                case Direction.North when key == KeyCode.D: return Direction.East;
                case Direction.North when key == KeyCode.W: return Direction.North;

                default:
                    throw new System.ArgumentException("��]�̓��͂��s���L�[�ł͂Ȃ�: " + key);
            }
        }

        /// <summary>
        /// ���͂ɉ����č��E�ǂ����90�x��]���邩
        /// </summary>
        public static float To90DegreeRotateAngle(this KeyCode key)
        {
            switch (key)
            {
                case KeyCode.A: return -90;
                case KeyCode.D: return 90;
                default:
                    throw new System.ArgumentException("��]�̓��͂��s���L�[�ł͂Ȃ�: " + key);
            }
        }

        /// <summary>
        /// �O��ړ�������ۂ̃L�[�ƌ����ɉ������A�O�������͌��̓Y����
        /// </summary>
        public static bool TryGetFrontAndBackIndex(this KeyCode key, Direction current, out Vector2Int index)
        {
            if (key == KeyCode.W) { index = current.ToIndex(); return true; }
            if (key == KeyCode.S) { index = -current.ToIndex(); return true; }

            index = default;
            return false;
        }
    }
}
