using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// インスペクター上で複数のオブジェクトに対して、同じ値を割り当てる必要があるものを定数で管理する。
    /// 一括で管理するため全シーン共通で使う。
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
    }
}
