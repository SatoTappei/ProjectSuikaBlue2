using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 試験的後退記録機能 : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] Transform _parent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject g = Instantiate(_prefab, _parent);
            g.GetComponentInChildren<Text>().text = Time.time.ToString();
        }
    }
}
