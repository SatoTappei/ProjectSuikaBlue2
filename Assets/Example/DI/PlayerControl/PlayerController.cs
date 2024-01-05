using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using UniRx.Triggers;

namespace PSB.DI.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CharacterController _controller;

        IInput _input;

        [Inject]
        void Construct(IInput input) // public •K—v‚È‚¢
        {
            _input = input;

            // OnEnable‚æ‚èŒã‚ÉŒÄ‚Î‚ê‚Ä‚¢‚éH
            // OnEnable‚É“o˜^ˆ—‚ð‘‚¢‚½Û‚É‚Ê‚é‚Û‚É‚È‚Á‚½B
            _input.InputEvent += Move;
            this.OnDestroyAsObservable().Subscribe(_ => _input.InputEvent -= Move);
        }

        void Move(InputDirections direction)
        {
            _controller.Move(DirectionToVector(direction) * Time.deltaTime);
        }

        Vector3 DirectionToVector(InputDirections direction)
        {
            if (direction == InputDirections.Forward) return Vector3.forward;
            else if (direction == InputDirections.Back) return Vector3.back;
            else if (direction == InputDirections.Left) return Vector3.left;
            else if (direction == InputDirections.Right) return Vector3.right;
            return Vector3.zero;
        }
    }
}
