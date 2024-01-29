using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// インスペクター上で複数のオブジェクトに対して、同じ値を割り当てる必要があるものを定数で管理する。
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// インゲームのシーン名
        /// </summary>
        public const string InGameSceneName = "InGame";
        /// <summary>
        /// キャラクターとの会話シーン名
        /// </summary>
        public const string CharacterSceneName = "Character3D";

        const string FootingLayerName = "Footing";
        const string HoleRangeLayerName = "HoleRange";
        const string StageBorderLayerName = "StageBorder";

        /// <summary>
        /// 接地するとプレイヤーが再度ジャンプ可能になるレイヤー
        /// </summary>
        public static int FootingLayer => 1 << LayerMask.NameToLayer(FootingLayerName);
        /// <summary>
        /// 穴の判定のレイヤー
        /// </summary>
        public static int HoleRangeLayer => 1 << LayerMask.NameToLayer(HoleRangeLayerName);
        /// <summary>
        /// ステージの端を判定するレイヤー
        /// </summary>
        public static int StageBorderLayer => 1 << LayerMask.NameToLayer(StageBorderLayerName);
    }
}
