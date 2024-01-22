using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath
{
    /// <summary>
    /// �������̔z���������̈ʒu����߂����Ƀ\�[�g����
    /// </summary>
    public static void Sort(Transform[] array, in Vector3 center)
    {
        QuickSort(array, 0, array.Length - 1, center);
    }

    static void QuickSort(Transform[] array, int left, int right, in Vector3 center)
    {
        if (right - left <= 1) return;

        float pivot = Vector3.SqrMagnitude(array[right].position - center);
        int insertIndex = left;
        for (int i = left; i < right; i++)
        {
            float dist = Vector3.SqrMagnitude(array[i].position - center);
            if (dist < pivot)
            {
                Swap(array, i, insertIndex++);
            }
        }

        Swap(array, insertIndex, right);
        QuickSort(array, left, insertIndex - 1, center);
        QuickSort(array, insertIndex + 1, right, center);
    }

    /// <summary>
    /// �N�C�b�N�\�[�g
    /// </summary>
    public static void Sort(params int[] array)
    {
        QuickSort(array, 0, array.Length - 1);
    }

    static void QuickSort(int[] array, int left, int right)
    {
        if (right - left <= 1) return;

        int pivot = array[right];
        int insertIndex = left;
        for (int i = left; i < right; i++)
        {
            if (array[i] < pivot)
            {
                Swap(array, i, insertIndex++);
            }
        }

        Swap(array, insertIndex, right);
        QuickSort(array, left, insertIndex - 1);
        QuickSort(array, insertIndex + 1, right);
    }

    static void Swap<T>(T[] array, int a, int b)
    {
        T temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }

    /// <summary>
    /// ���`�⊮
    /// </summary>
    public static Vector3 Lerp(in Vector3 a, in Vector3 b, float t)
    {
        // t��0~1�̊ԂɃN�����v����
        if (t < 0) t = 0;
        if (t > 1.0f) t = 1.0f;

        return a + t * (b - a);
    }

    /// <summary>
    /// ����
    /// </summary>
    public static float Dot(in Vector3 a, in Vector3 b)
    {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    /// <summary>
    /// �O��
    /// </summary>
    public static Vector3 Cross(in Vector3 a, in Vector3 b)
    {
        float x = (a.y * b.z) - (a.z * b.y);
        float y = (a.z * b.x) - (a.x * b.z);
        float z = (a.x * b.y) - (a.y * b.x);
        
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// �x�N�g���̐��K��
    /// </summary>
    public static Vector3 Normalize(in Vector3 v)
    {
        return v / Magnitude(v);
    }

    /// <summary>
    /// �x�N�g���̒���
    /// </summary>
    public static float Magnitude(in Vector3 v)
    {
        float f = (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
        // ���[�g�̌v�Z���͉̂\�����A���x���d�v�Ȃ̂Ŋ����̂��̂��g���B
        return Mathf.Sqrt(f);
    }

    /// <summary>
    /// �N�H�[�^�j�I��
    /// ���̊e������0~1�A�p�x�̓I�C���[�p
    /// </summary>
    public static Quaternion Quat(Vector3 axis, float euler)
    {
        euler = euler / 360 * Mathf.PI * 2;
        float x = axis.x * Mathf.Sin(euler / 2);
        float y = axis.y * Mathf.Sin(euler / 2);
        float z = axis.z * Mathf.Sin(euler / 2);
        float w = Mathf.Cos(euler / 2);

        return new Quaternion(x, y, z, w);
    }
}
