using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerDirectionalLight : MonoBehaviour
{
    [Header("Start時にオフにする")]
    [SerializeField] GameObject _light;

    void Start()
    {
        // ゲーム開始時にライトをオフにした場合と
        // ヒエラルキーでライトをオフにした場合でライティングが変わってしまう。
        _light.SetActive(false);
    }
}
