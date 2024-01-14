using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    [SerializeField] float _speed = 1;

    Transform _t;

    void Start()
    {
        _t = transform;
        _t.eulerAngles = new Vector3(0, Random.value * 360.0f, 0);
    }

    void Update()
    {
        _t.eulerAngles += new Vector3(0, _speed * Time.deltaTime, 0);
    }
}
