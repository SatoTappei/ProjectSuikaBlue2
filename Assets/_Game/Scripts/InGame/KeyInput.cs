using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    // キー入力でゲームを動かすデバッグ用
    public class KeyInput : MonoBehaviour
    {
        [Header("シーン単体で動かすのでカメラが必要")]
        [SerializeField] Camera _mainCamera;

        void Start()
        {           
            _mainCamera.gameObject.SetActive(true);
        }

        void Update()
        {
            PlayerControlMessage msg = new();
            bool isInput = false;
            if (Input.GetKey(KeyCode.A)) { msg.KeyDownA = true; isInput = true; }
            if (Input.GetKey(KeyCode.D)) { msg.KeyDownD = true; isInput = true; }
            if (Input.GetKey(KeyCode.Space)) { msg.KeyDownSpace = true; isInput = true; }

            if (isInput) MessageBroker.Default.Publish(msg);
        }
    }
}