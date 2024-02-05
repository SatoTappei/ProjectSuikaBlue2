using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "DungeonParameterSettings_")]
    public class DungeonParameterSettings : ScriptableObject
    {
        [Header("ダンジョン生成設定")]
        [SerializeField] int _size = 20;
        [SerializeField] float _cellSize = 1;
        [SerializeField] Vector2Int _entry;
        [Header("WFC設定")]
        [Min(1)]
        [SerializeField] uint _wfcSeed = 1;
        [SerializeField] bool _wfcRandomSeed;
        [Header("自己回避ウォーク設定")]
        [Min(1)]
        [SerializeField] uint _walkSeed = 1;
        [SerializeField] bool _walkRandomSeed;
        [Header("セル間のレイキャストの設定")]
        [SerializeField] Vector3 _cellRayOffset = Vector3.up;
        [SerializeField] int _cellRayHitMax = 9;
        [Header("更新速度")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        /// <summary>
        /// ダンジョンの1辺のセル数
        /// </summary>
        public int Size => _size;
        /// <summary>
        /// セルの大きさ
        /// </summary>
        public float CellSize => _cellSize;
        /// <summary>
        /// ダンジョンの入口の位置
        /// </summary>
        public Vector2Int Entry => _entry;
        /// <summary>
        /// WFCアルゴリズムのシード値
        /// </summary>
        public uint WfcSeed => _wfcSeed;
        /// <summary>
        /// WFCアルゴリズムでランダムなシード値を使うかどうか
        /// </summary>
        public bool WfcRandomSeed => _wfcRandomSeed;
        /// <summary>
        /// 自己回避移動アルゴリズムのシード値
        /// </summary>
        public uint WalkSeed => _walkSeed;
        /// <summary>
        /// 自己回避移動アルゴリズムでランダムなシード値を使うかどうか
        /// </summary>
        public bool WalkRandomSeed => _walkRandomSeed;
        /// <summary>
        /// セル間のレイキャストを行う際のセルの中心からのオフセット
        /// </summary>
        public Vector3 CellRayOffset => _cellRayOffset;
        /// <summary>
        /// セル間のレイキャストが判定する最大個数
        /// </summary>
        public int CellRayHitMax => _cellRayHitMax;
        /// <summary>
        /// アルゴリズムの更新速度
        /// </summary>
        public float StepSpeed => _stepSpeed;
    }
}
