using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {

    }

    void AngleAxis(float θ, Vector3 axis)
    {
        float x = axis.x * Mathf.Sin(θ / 2);
        float y = axis.y * Mathf.Sin(θ / 2);
        float z = axis.z * Mathf.Sin(θ / 2);
        float w = Mathf.Cos(θ / 2);

        // バグってる。2つの値が違う。
        Debug.Log(Quaternion.AngleAxis(30, Vector3.forward));
        Debug.Log(new Quaternion(x, y, z, w));
    }

    void Matrix()
    {

    }
}
