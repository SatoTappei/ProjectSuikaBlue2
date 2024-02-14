using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class Vector2IntExtensions
    {
        /// <summary>
        /// 2点間のチェビシェフ距離
        /// </summary>
        public static float ChebyshevDistance(this Vector2Int a, Vector2Int b)
        {
            // マンハッタン距離
            int x = Mathf.Abs(a.x - b.x);
            int y = Mathf.Abs(a.y - b.y);

            return Mathf.Max(x, y);
        }

        /// <summary>
        /// 対応した方向を返す。
        /// </summary>
        public static Direction ToDirection(this Vector2Int direction)
        {
            if (direction == Vector2Int.right) return Direction.East;
            if (direction == Vector2Int.left) return Direction.West;
            if (direction == Vector2Int.up) return Direction.North;
            if (direction == Vector2Int.down) return Direction.South;

            throw new System.ArgumentException("東西南北以外の値: " + direction);
        }
    }
}
