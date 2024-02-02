using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFCアルゴリズム周りは専用の名前空間にまとめてある。
using PSB.Game.SAW; // 自己回避ウォークアルゴリズムは専用の名前空間にまとめてある。

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
        [SerializeField] uint _wfcSeed = 1;
        [SerializeField] bool _wfcRandomSeed;
        [Header("自己回避ウォーク設定")]
        [Min(1)]
        [SerializeField] uint _walkSeed = 1;
        [SerializeField] bool _walkRandomSeed;
        [Header("更新速度")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Dungeon _dungeon;
        SelfAvoidingWalk _walk;

        void Awake()
        {
            // タイルを生成してそれにダンジョンのグリッドを合わせる。
            // 2つのタイルの間に1つのセルが存在するので、サイズそのままだと1つ足りなくなってしまう。
            _dungeon = new(_size - 1, _size - 1, _cellSize);
            uint seed = _walkRandomSeed ? (uint)Random.Range(1, uint.MaxValue) : _walkSeed;
            _walk = new(_size - 1, _size - 1, _cellSize, seed);
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            //await BuildDungeonBaseAsync(token);
            await CreatePathAsync(token);



            // 例外が出るかテスト用
            //await UniTask.Yield();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(
            //    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            // ダンジョン生成に使うクラスは完成したらメモリを解放しても良いかもしれない
        }

        // 波動関数の崩壊アルゴリズムでベースとなるダンジョンを作る
        async UniTask BuildDungeonBaseAsync(CancellationToken token)
        {
            // シード値を決めて生成
            uint seed = _wfcRandomSeed ? (uint)Random.Range(1, uint.MaxValue) : _wfcSeed;
            Logic logic = new(_size, _size, seed);

            for (int i = 0; i < _size * _size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        // 自己回避ウォークアルゴリズムでスタートからゴールへ一本道を作る
        async UniTask CreatePathAsync(CancellationToken token)
        {
            for (int i = 0; i < (_size - 1) * (_size - 1); i++)
            {
                _walk.Step();
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
            if (_walk != null) _walk.DrawGridOnGizmos();
        }
    }
}
