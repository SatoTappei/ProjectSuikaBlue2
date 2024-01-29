using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.GameExample
{
    // キー入力でゲームを動かすデバッグ用
    public class KeyInput : MonoBehaviour
    {
        void Update()
        {
            PSB.Game.KeyInputMessage msg = new();
            bool isInput = false;
            if (Input.GetKey(KeyCode.A)) { msg.KeyDownA = true; isInput = true; }
            if (Input.GetKey(KeyCode.D)) { msg.KeyDownD = true; isInput = true; }
            if (Input.GetKey(KeyCode.Space)) { msg.KeyDownSpace = true; isInput = true; }

            if (isInput) MessageBroker.Default.Publish(msg);
        }
    }
}