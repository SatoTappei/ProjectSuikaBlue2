using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.DI.PlayerControl
{
    public enum InputDirections
    {
        Forward,
        Back,
        Left,
        Right,
    }

    public interface IInput
    {
        event UnityAction<InputDirections> InputEvent;
    }
}