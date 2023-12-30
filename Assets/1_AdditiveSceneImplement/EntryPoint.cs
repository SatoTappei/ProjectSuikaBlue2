using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Architect
{
    public class EntryPoint : MonoBehaviour
    {
        void Start()
        {
            // Managerシーンにあるインスタンスを参照する
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Check();
            }
        }
    }
}