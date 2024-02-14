using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB
{
    /// <summary>
    /// 自作の数学クラス
    /// </summary>
    public static class MyMath
    {
        /// <summary>
        /// 線形補完
        /// </summary>
        public static Vector3 Lerp(in Vector3 a, in Vector3 b, float t)
        {
            // tを0~1の間にクランプする
            if (t < 0) t = 0;
            if (t > 1.0f) t = 1.0f;

            return a + t * (b - a);
        }

        /// <summary>
        /// 内積
        /// </summary>
        public static float Dot(in Vector3 a, in Vector3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        /// <summary>
        /// 外積
        /// </summary>
        public static Vector3 Cross(in Vector3 a, in Vector3 b)
        {
            float x = (a.y * b.z) - (a.z * b.y);
            float y = (a.z * b.x) - (a.x * b.z);
            float z = (a.x * b.y) - (a.y * b.x);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// ベクトルの正規化
        /// </summary>
        public static Vector3 Normalize(in Vector3 v)
        {
            return v / Magnitude(v);
        }

        /// <summary>
        /// ベクトルの長さ
        /// </summary>
        public static float Magnitude(in Vector3 v)
        {
            // ルートの計算自体は複雑かつ精度が重要なので既存のものを使う。
            return Mathf.Sqrt(SqrMagnitude(v));
        }

        /// <summary>
        /// ベクトルの2乗の長さ
        /// </summary>
        public static float SqrMagnitude(in Vector3 v)
        {
            return (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
        }

        /// <summary>
        /// クォータニオン
        /// 軸の各成分は0~1、角度はオイラー角
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
