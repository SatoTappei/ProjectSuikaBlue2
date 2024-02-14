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
        public static Vector3 ToDirectionVector(this KeyCode key, Direction current)
        {
            if (key == KeyCode.W) return current.ToVector3();
            if (key == KeyCode.S) return -current.ToVector3();

            throw new System.ArgumentException("移動の入力を行うキーではない: " + key);
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
        /// 前後移動をする際のキーと向きに応じた、前もしくは後ろの添え字
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
