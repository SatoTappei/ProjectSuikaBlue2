using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace PSB.WaveFunctionCollapse
{
    public class Provider : MonoBehaviour
    {
        [SerializeField] View _view;
        [SerializeField] int _size = 20;
        [SerializeField] uint _seed = 0;
        [SerializeField] bool _randomSeed;
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Logic _logic;

        void Start()
        {
            Init();
            StartCoroutine(CollapseMap());
        }

        void Init()
        {
            uint seed = _randomSeed ? (uint)Random.Range(uint.MinValue, uint.MaxValue) : _seed;
            _logic = new(_size, _size, seed);
        }

        IEnumerator CollapseMap()
        {
            for (int i = 0; i < _size * _size; i++)
            {
                _view.Create(_logic.Step());
                yield return new WaitForSeconds(_stepSpeed);
            }
        }
    }
}
