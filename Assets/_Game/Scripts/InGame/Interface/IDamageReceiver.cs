using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// ダメージを受けるオブジェクトに実装
    /// </summary>
    public interface IDamageReceiver
    {
        /// <summary>
        /// 引数の値のダメージを受ける
        /// </summary>
        public void Damage();
    }
}
