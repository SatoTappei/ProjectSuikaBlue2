using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class Vector2IntExtensions
    {
        /// <summary>
        /// 2�_�Ԃ̃`�F�r�V�F�t����
        /// </summary>
        public static float ChebyshevDistance(this Vector2Int a, Vector2Int b)
        {
            // �}���n�b�^������
            int x = Mathf.Abs(a.x - b.x);
            int y = Mathf.Abs(a.y - b.y);

            return Mathf.Max(x, y);
        }

        /// <summary>
        /// �Ή�����������Ԃ��B
        /// </summary>
        public static Direction ToDirection(this Vector2Int direction)
        {
            if (direction == Vector2Int.right) return Direction.East;
            if (direction == Vector2Int.left) return Direction.West;
            if (direction == Vector2Int.up) return Direction.North;
            if (direction == Vector2Int.down) return Direction.South;

            throw new System.ArgumentException("������k�ȊO�̒l: " + direction);
        }
    }
}
