using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;

namespace PSB.DI.PlayerControl
{
    /// <summary>
    /// ���t���[�����͂��󂯎��B
    /// ���͂ɉ�����������������ꍇ�͂��̃N���X�𒍓����A�R�[���o�b�N��o�^����B
    /// </summary>
    public class KeyboardInput : IInput, ITickable
    {
        public event UnityAction<InputDirections> InputEvent;

        void ITickable.Tick()
        {
            if (Input.GetKey(KeyCode.W)) InputEvent?.Invoke(InputDirections.Forward);
            if (Input.GetKey(KeyCode.S)) InputEvent?.Invoke(InputDirections.Back);
            if (Input.GetKey(KeyCode.A)) InputEvent?.Invoke(InputDirections.Left);
            if (Input.GetKey(KeyCode.D)) InputEvent?.Invoke(InputDirections.Right);
        }
    }
}
