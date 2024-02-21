using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Buffers;
using System;

namespace PSB.Game
{
    public static class Utility
    {
        /// <summary>
        /// 引数の添え字が配列の範囲内かどうかを調べる
        /// </summary>
        public static bool CheckInLength<T>(T[,] array, int y, int x)
        {
            return 0 <= y && y < array.GetLength(0) && 0 <= x && x < array.GetLength(1);
        }

        /// <summary>
        /// 引数の添え字が配列の範囲内かどうかを調べる
        /// </summary>
        public static bool CheckInLength<T>(T[,] array, Vector2Int index)
        {
            return CheckInLength(array, index.y, index.x);
        }

        /// <summary>
        /// 上下左右をある程度ランダムな順番で返す
        /// </summary>
        public static IEnumerable<Vector2Int> RandomDirection(uint seed)
        {
            Vector2Int[] a = ArrayPool<Vector2Int>.Shared.Rent(4);
            a[0] = Vector2Int.up;
            a[1] = Vector2Int.down;
            a[2] = Vector2Int.left;
            a[3] = Vector2Int.right;

            int r = new Unity.Mathematics.Random(seed).NextInt(0, 4);
            for (int i = 0; i < 4; i++)
            {
                yield return a[r];
                r++;
                r %= 4;
            }

            Array.Clear(a, 0, a.Length);
            ArrayPool<Vector2Int>.Shared.Return(a);
        }

        /// <summary>
        /// UnityEngineのRandomを用いて1からuint型の最大値をランダムに返す
        /// </summary>
        public static uint RandomSeed()
        {
            return (uint)UnityEngine.Random.Range(1, uint.MaxValue);
        }

        /// <summary>
        /// 0から引数の値の範囲でランダムなint型2つの添え字を返す。
        /// </summary>
        public static Vector2Int RandomIndex(int max)
        {
            int a = UnityEngine.Random.Range(0, max);
            int b = UnityEngine.Random.Range(0, max);
            return new Vector2Int(a, b);
        }
    }
}
