using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {

    }

    void AngleAxis(float ��, Vector3 axis)
    {
        float x = axis.x * Mathf.Sin(�� / 2);
        float y = axis.y * Mathf.Sin(�� / 2);
        float z = axis.z * Mathf.Sin(�� / 2);
        float w = Mathf.Cos(�� / 2);

        // �o�O���Ă�B2�̒l���Ⴄ�B
        Debug.Log(Quaternion.AngleAxis(30, Vector3.forward));
        Debug.Log(new Quaternion(x, y, z, w));
    }

    void Matrix()
    {

    }
}
