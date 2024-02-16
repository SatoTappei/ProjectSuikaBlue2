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
        /// 方向に対応したクォータニオンを返す。
        /// </summary>
        public static Quaternion ToQuaternion(this Direction direction)
        {
            if (direction == Direction.East) return Quaternion.Euler(0, 90, 0);
            if (direction == Direction.West) return Quaternion.Euler(0, -90, 0);
            if (direction == Direction.North) return Quaternion.identity;
            if (direction == Direction.South) return Quaternion.Euler(0, 180, 0);

            throw new System.ArgumentException("東西南北以外の列挙型: " + direction);
        }

        /// <summary>
        /// 現在向いている方向から上下左右に応じた回転を行った後の方向
        /// </summary>
        public static Direction TurnedDirection(this Direction current, Arrow arrow)
        {
            // 上の場合はその方向のままなので向きは変わらない
            // 下の場合は反対方向に向く
            // 左の場合は反時計回りに90度回転する
            // 右の場合は時計回りに90度回転する

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
                    throw new System.ArgumentException("上下左右以外の列挙型: " + arrow);
            }
        }
    }
}
