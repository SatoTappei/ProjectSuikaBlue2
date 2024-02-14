using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PSB.Game
{
    public class KeyInput : MonoBehaviour
    {
        void Update()
        {
            KeyInputMessage msg = new();
            if (Input.GetKey(KeyCode.A)) msg.KeyDownA = true;
            if (Input.GetKey(KeyCode.S)) msg.KeyDownS = true;
            if (Input.GetKey(KeyCode.D)) msg.KeyDownD = true;
            if (Input.GetKey(KeyCode.W)) msg.KeyDownW = true;
            if (Input.GetKey(KeyCode.Space)) msg.KeyDownSpace = true;

            if (msg.KeyDownA || msg.KeyDownS || msg.KeyDownD || msg.KeyDownW || msg.KeyDownSpace)
            {
                MessageBroker.Default.Publish(msg);
            }
        }
    }
}
