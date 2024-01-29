using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFCアルゴリズム周りは専用の名前空間にまとめてある

namespace PSB.Game
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] TileBuilder _tileBuilder;
        [Header("生成設定")]
        [SerializeField] int _size = 20;
        [Min(1)]
        [SerializeField] uint _seed = 1;
        [SerializeField] bool _randomSeed;
        [Header("更新速度")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // シード値を決めて生成
            uint seed = _randomSeed ? (uint)Random.Range(1, uint.MaxValue) : _seed;
            Logic logic = new(_size, _size, seed);

            for (int i = 0; i < _size * _size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }
    }
}
