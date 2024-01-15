using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PSB.Game
{
    public class StageMonitor : MonoBehaviour
    {
        [Header("ゴールの方角の判定用")]
        [SerializeField] Transform _player;
        [SerializeField] Transform _goal;
        [Header("コインの数判定用")]
        [SerializeField] GameObject[] _coins;

        GameState _gameState;

        [Inject]
        void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        void Update()
        {
            Vector3 dir = _goal.position - _player.position;
            if (0 < dir.x && 0 < dir.y) _gameState.GoalDirection = Direction.UpperRight;
            else if (dir.x < 0 && 0 < dir.y) _gameState.GoalDirection = Direction.UpperLeft;
            else if (0 < dir.x && dir.y < 0) _gameState.GoalDirection = Direction.LowerRight;
            else if (dir.x < 0 && dir.y < 0) _gameState.GoalDirection = Direction.LowerLeft;
        }
    }
}
