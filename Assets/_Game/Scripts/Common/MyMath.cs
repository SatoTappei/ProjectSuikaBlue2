using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB
{
    /// <summary>
    /// ����̐��w�N���X
    /// </summary>
    public static class MyMath
    {
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
            // ���[�g�̌v�Z���͕̂��G�����x���d�v�Ȃ̂Ŋ����̂��̂��g���B
            return Mathf.Sqrt(SqrMagnitude(v));
        }

        /// <summary>
        /// �x�N�g����2��̒���
        /// </summary>
        public static float SqrMagnitude(in Vector3 v)
        {
            return (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
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
}
