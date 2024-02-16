using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class DirectionExtensions
    {
        /// <summary>
        /// �����ɑΉ������Y������Ԃ��B
        /// </summary>
        public static Vector2Int ToIndex(this Direction direction)
        {
            if (direction == Direction.East) return Vector2Int.right;
            if (direction == Direction.West) return Vector2Int.left;
            if (direction == Direction.North) return Vector2Int.up;
            if (direction == Direction.South) return Vector2Int.down;

            throw new System.ArgumentException("������k�ȊO�̗񋓌^: " + direction);
        }

        /// <summary>
        /// �����ɑΉ������P�ʃx�N�g����Ԃ��B
        /// </summary>
        public static Vector3 ToVector3(this Direction direction)
        {
            if (direction == Direction.East) return Vector3.right;
            if (direction == Direction.West) return Vector3.left;
            if (direction == Direction.North) return Vector3.forward;
            if (direction == Direction.South) return Vector3.back;

            throw new System.ArgumentException("������k�ȊO�̗񋓌^: " + direction);
        }

        /// <summary>
        /// �����ɑΉ������N�H�[�^�j�I����Ԃ��B
        /// </summary>
        public static Quaternion ToQuaternion(this Direction direction)
        {
            if (direction == Direction.East) return Quaternion.Euler(0, 90, 0);
            if (direction == Direction.West) return Quaternion.Euler(0, -90, 0);
            if (direction == Direction.North) return Quaternion.identity;
            if (direction == Direction.South) return Quaternion.Euler(0, 180, 0);

            throw new System.ArgumentException("������k�ȊO�̗񋓌^: " + direction);
        }

        /// <summary>
        /// ���݌����Ă����������㉺���E�ɉ�������]���s������̕���
        /// </summary>
        public static Direction TurnedDirection(this Direction current, Arrow arrow)
        {
            // ��̏ꍇ�͂��̕����̂܂܂Ȃ̂Ō����͕ς��Ȃ�
            // ���̏ꍇ�͔��Ε����Ɍ���
            // ���̏ꍇ�͔����v����90�x��]����
            // �E�̏ꍇ�͎��v����90�x��]����

            switch (current)
            {
                case Direction.East when arrow == Arrow.Left: return Direction.North;
                case Direction.East when arrow == Arrow.Down: return Direction.West;
                case Direction.East when arrow == Arrow.Right: return Direction.South;
                case Direction.East when arrow == Arrow.Up: return Direction.East;

                case Direction.West when arrow == Arrow.Left: return Direction.South;
                case Direction.West when arrow == Arrow.Down: return Direction.East;
                case Direction.West when arrow == Arrow.Right: return Direction.North;
                case Direction.West when arrow == Arrow.Up: return Direction.West;

                case Direction.South when arrow == Arrow.Left: return Direction.East;
                case Direction.South when arrow == Arrow.Down: return Direction.North;
                case Direction.South when arrow == Arrow.Right: return Direction.West;
                case Direction.South when arrow == Arrow.Up: return Direction.South;

                case Direction.North when arrow == Arrow.Left: return Direction.West;
                case Direction.North when arrow == Arrow.Down: return Direction.South;
                case Direction.North when arrow == Arrow.Right: return Direction.East;
                case Direction.North when arrow == Arrow.Up: return Direction.North;

                default:
                    throw new System.ArgumentException("�㉺���E�ȊO�̗񋓌^: " + arrow);
            }
        }
    }
}
