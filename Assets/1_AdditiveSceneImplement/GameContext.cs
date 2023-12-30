using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class GameContext
    {
        ReactiveProperty<bool> _isPlayerInput = new();
        ReactiveProperty<bool> _isPause = new();

        public IReadOnlyReactiveProperty<bool> IsPlayerInputObservable => _isPlayerInput;
        public IReadOnlyReactiveProperty<bool> IsPauseObservable => _isPause;

        public bool IsPlayerInput { get => _isPlayerInput.Value; set => _isPlayerInput.Value = value; }
        public bool IsPause { get => _isPause.Value; set => _isPause.Value = value; }
    }
}
