using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB
{
    /// <summary>
    /// 自作のソート機能クラス
    /// </summary>
    public static class MySort
    {
        /// <summary>
        /// クイックソート
        /// </summary>
        public static void Sort<T>(params T[] array) where T : IComparable<T>
        {
            QuickSort(array, 0, array.Length - 1);
        }

        static void QuickSort<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            // ソートする配列のサイズが1以下
            if (right <= left) return;

            // 一番右の値以下の場合は挿入位置の値と入れ替える
            // 入れ替える度に挿入位置を右に1つずらす
            T pivot = array[right];
            int insert = left;
            for (int i = left; i < right; i++)
            {
                if (array[i].CompareTo(pivot) < 0)
                {
                    (array[i], array[insert]) = (array[insert], array[i]);
                    insert++;
                }
            }

            // 挿入位置の値と一番右の値を入れ替える
            (array[insert], array[right]) = (array[right], array[insert]);

            // 半分にして再帰
            QuickSort(array, left, insert - 1);
            QuickSort(array, insert + 1, right);
        }
    }
}
