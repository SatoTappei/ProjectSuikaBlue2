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
        List<KeyCode> _inputs = new List<KeyCode>();

        void Update()
        {
            _inputs.Clear();

            if (Input.GetKeyDown(KeyCode.A)) _inputs.Add(KeyCode.A);
            if (Input.GetKeyDown(KeyCode.S)) _inputs.Add(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.D)) _inputs.Add(KeyCode.D);
            if (Input.GetKeyDown(KeyCode.W)) _inputs.Add(KeyCode.W);
            if (Input.GetKeyDown(KeyCode.Space)) _inputs.Add(KeyCode.Space);

            if (_inputs.Count > 0)
            {
                MessageBroker.Default.Publish(new PlayerControlMessage() { InputKeys = _inputs });
                Debug.Log("送信: " + string.Join(",", _inputs));
            }
        }
    }
}
