using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PSB.WaveFunctionCollapse
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] View _view;
        [SerializeField] int _size = 20;
        [Min(1)]
        [SerializeField] uint _seed = 1;
        [SerializeField] bool _randomSeed;
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        void Start()
        {
            Logic logic = InitLogic();
            StartCoroutine(CollapseMapCoroutine(logic));
        }

        Logic InitLogic()
        {
            uint seed = _randomSeed ? (uint)Random.Range(1, uint.MaxValue) : _seed;
            return new(_size, _size, seed);
        }

        IEnumerator CollapseMapCoroutine(Logic logic)
        {
            for (int i = 0; i < _size * _size; i++)
            {
                _view.Draw(logic.Step());
                yield return new WaitForSeconds(_stepSpeed);
            }
        }
    }
}
