using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class DirectionExtensions
    {
        /// <summary>
        /// 方向に対応した添え字を返す。
        /// </summary>
        public static Vector2Int ToIndex(this Direction direction)
        {
            if (direction == Direction.East) return Vector2Int.right;
            if (direction == Direction.West) return Vector2Int.left;
            if (direction == Direction.North) return Vector2Int.up;
            if (direction == Direction.South) return Vector2Int.down;

            throw new System.ArgumentException("東西南北以外の列挙型: " + direction);
        }

        /// <summary>
        /// 方向に対応した単位ベクトルを返す。
        /// </summary>
        public static Vector3 ToVector3(this Direction direction)
        {
            if (direction == Direction.East) return Vector3.right;
            if (direction == Direction.West) return Vector3.left;
            if (direction == Direction.North) return Vector3.forward;
            if (direction == Direction.South) return Vector3.back;

            throw new System.ArgumentException("東西南北以外の列挙型: " + direction);
        }

        /// <summary>
        /// 方向に対応した単位ベクトルを返す。
        /// </summary>
        public static Quaternion ToQuaternion(this Direction direction)
        {
            if (direction == Direction.East) return Quaternion.Euler(0, 90, 0);
            if (direction == Direction.West) return Quaternion.Euler(0, -90, 0);
            if (direction == Direction.North) return Quaternion.identity;
            if (direction == Direction.South) return Quaternion.Euler(0, 180, 0);

            throw new System.ArgumentException("東西南北以外の列挙型: " + direction);
        }
    }
}
