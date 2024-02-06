using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class KeyCodeExtensions
    {
        /// <summary>
        /// 指定した方向を向いている状態での入力時に前後移動をする単位方向ベクトル
        /// </summary>
        public static Vector3 ToNormalizedDirectionVector(this KeyCode key, Direction current)
        {
            switch (current)
            {
                case Direction.East when key == KeyCode.W: return Vector3.right;
                case Direction.East when key == KeyCode.S: return Vector3.left;

                case Direction.West when key == KeyCode.W: return Vector3.left;
                case Direction.West when key == KeyCode.S: return Vector3.right;

                case Direction.South when key == KeyCode.W: return Vector3.back;
                case Direction.South when key == KeyCode.S: return Vector3.forward;

                case Direction.North when key == KeyCode.W: return Vector3.forward;
                case Direction.North when key == KeyCode.S: return Vector3.back;

                default:
                    throw new System.ArgumentException("移動の入力を行うキーではない: " + key);
            }
        }

        /// <summary>
        /// 現在向いている方向から入力したキーに応じた回転を行った後の方向
        /// </summary>
        public static Direction ToTurnedDirection(this KeyCode key, Direction current)
        {
            // Wキーの場合はその方向に移動するので向きは変わらない
            // Sキーの場合は反対方向に向く
            // Aキーの場合は反時計回りに90度回転する
            // Dキーの場合は時計回りに90度回転する

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
                    throw new System.ArgumentException("回転の入力を行うキーではない: " + key);
            }
        }

        /// <summary>
        /// 入力に応じて左右どちらに90度回転するか
        /// </summary>
        public static float To90DegreeRotateAngle(this KeyCode key)
        {
            switch (key)
            {
                case KeyCode.A: return -90;
                case KeyCode.D: return 90;
                default:
                    throw new System.ArgumentException("回転の入力を行うキーではない: " + key);
            }
        }

        /// <summary>
        /// 前後移動をする際のキーと向きに応じた、前もしくは後ろの添え字を指す向きを返す
        /// </summary>
        public static bool TryGetFrontAndBackIndexDirection(this KeyCode key, Direction current, out Vector2Int direction)
        {
            if (key == KeyCode.W)
            {
                if (current == Direction.East) { direction = Vector2Int.right; return true; }
                if (current == Direction.West) { direction = Vector2Int.left; return true; }
                if (current == Direction.North) { direction = Vector2Int.up; return true; }
                if (current == Direction.South) { direction = Vector2Int.down; return true; }
            }
            if (key == KeyCode.S)
            {
                if (current == Direction.East) { direction = Vector2Int.left; return true; }
                if (current == Direction.West) { direction = Vector2Int.right; return true; }
                if (current == Direction.North) { direction = Vector2Int.down; return true; }
                if (current == Direction.South) { direction = Vector2Int.up; return true; }
            }

            direction = default;
            return false;
        }
    }
}
