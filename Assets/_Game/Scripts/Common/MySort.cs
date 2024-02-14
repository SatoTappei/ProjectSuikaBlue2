using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB
{
    /// <summary>
    /// ����̃\�[�g�@�\�N���X
    /// </summary>
    public static class MySort
    {
        /// <summary>
        /// �N�C�b�N�\�[�g
        /// </summary>
        public static void Sort<T>(params T[] array) where T : IComparable<T>
        {
            QuickSort(array, 0, array.Length - 1);
        }

        static void QuickSort<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            // �\�[�g����z��̃T�C�Y��1�ȉ�
            if (right <= left) return;

            // ��ԉE�̒l�ȉ��̏ꍇ�͑}���ʒu�̒l�Ɠ���ւ���
            // ����ւ���x�ɑ}���ʒu���E��1���炷
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

            // �}���ʒu�̒l�ƈ�ԉE�̒l�����ւ���
            (array[insert], array[right]) = (array[right], array[insert]);

            // �����ɂ��čċA
            QuickSort(array, left, insert - 1);
            QuickSort(array, insert + 1, right);
        }
    }
}
