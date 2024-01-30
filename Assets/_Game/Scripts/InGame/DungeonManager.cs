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
        [Header("ダンジョン生成設定")]
        [SerializeField] int _size = 20;
        [SerializeField] float _cellSize = 1;
        [Header("WFC設定")]
        [Min(1)]
        [SerializeField] uint _seed = 1;
        [SerializeField] bool _randomSeed;
        [Header("更新速度")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Dungeon _dungeon;

        void Awake()
        {
            // タイルを生成してそれにダンジョンのグリッドを合わせる。
            // 2つのタイルの間に1つのセルが存在するので、サイズそのままだと1つ足りなくなってしまう。
            _dungeon = new(_size - 1, _size - 1, _cellSize);
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            _dungeon.CheckCell(1, 0);
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

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
        }
    }
}
