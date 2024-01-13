using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Quadratic
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] Vector2 _pointA;
        [SerializeField] Vector2 _pointB;
        [SerializeField] Vector2 _pointC;

        void Start()
        {
            Quadratic quad = new(_pointA, _pointB, _pointC);

            foreach (Vector2 v in Position())
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.transform.position = v;
            }

            for (float x = 0; x <= 100; x += 0.1f)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = new Vector3(x, quad.GetY(x), 0);
                g.transform.localScale = Vector3.one * 0.5f;
            }
        }

        IEnumerable<Vector2> Position()
        {
            yield return _pointA;
            yield return _pointB;
            yield return _pointC;
        }
    }
}
