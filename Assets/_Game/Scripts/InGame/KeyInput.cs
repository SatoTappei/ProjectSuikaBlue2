using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    // �L�[���͂ŃQ�[���𓮂����f�o�b�O�p
    public class KeyInput : MonoBehaviour
    {
        [Header("�V�[���P�̂œ������̂ŃJ�������K�v")]
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