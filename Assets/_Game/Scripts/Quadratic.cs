using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// 3つの座標を通る二次関数を求める。
    /// </summary>
    public class Quadratic
    {
        struct Equation
        {
            public float Y;
            public float XX;
            public float X;
        }

        Vector2 _p;
        Vector2 _q;
        Vector2 _r;
        float _a;
        float _b;
        float _c;

        public Quadratic(Vector2 p, Vector2 q, Vector2 r)
        {
            Function(p, q, r);
        }

        /// <summary>
        /// 二次関数を求める。
        /// </summary>
        public void Function(Vector2 p, Vector2 q, Vector2 r)
        {
            _p = p;
            _q = q;
            _r = r;

            Calculate();
        }

        /// <summary>
        /// 二次関数のxに対応したyを求める。
        /// </summary>
        public float GetY(float x)
        {
            return _a * x * x + _b * x + _c;
        }

        void Calculate()
        {
            Equation ea = Convert(_p);
            Equation eb = Convert(_q);
            Equation ec = Convert(_r);

            Equation ed = Difference(ea, eb);
            Equation ee = Difference(eb, ec);

            (float a, float b) ab = SolveQuadratic(ed, ee);

            _a = ab.a;
            _b = ab.b;
            _c = ea.Y - ea.XX * ab.a - ea.X * ab.b;
        }

        // y = ax^2 + bx + c
        Equation Convert(Vector2 point)
        {
            return new Equation() { Y = point.y, XX = point.x * point.x, X = point.x };
        }

        //   y =   ax^2 + bx + c
        // - y = - ax^2 - bx - c
        Equation Difference(Equation a, Equation b)
        {
            return new Equation() { Y = a.Y - b.Y, XX = a.XX - b.XX, X = a.X - b.X };
        }

        // 2つの式からaとbを求める
        (float a, float b) SolveQuadratic(Equation e, Equation f)
        {
            float ex = Mathf.Abs(e.X);
            float fx = Mathf.Abs(f.X);

            float lcm = ex * fx / Gcd(ex, fx);

            float m = lcm / ex;
            float n = lcm / fx;

            e.Y *= m;
            e.XX *= m;
            e.X *= m;

            f.Y *= n;
            f.XX *= n;
            f.X *= n;

            int sign = Mathf.Approximately(e.X, f.X) ? -1 : 1;

            Equation g = new Equation()
            {
                Y = e.Y + f.Y * sign,
                XX = e.XX + f.XX * sign,
                X = 0,
            };

            float a = g.Y / g.XX;
            float b = (e.Y - e.XX * a) / e.X;

            return (a, b);
        }

        float Gcd(float a, float b) => b == 0 ? a : Gcd(b, a % b);
    }
}
