using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// キャラクターが任意のタイミングで作用可能なオブジェクトに実装
    /// </summary>
    public interface IInteractive
    {
        /// <summary>
        /// 適当な投げっぱなしの処理
        /// </summary>
        public void Action(object arg = null);
    }
}
