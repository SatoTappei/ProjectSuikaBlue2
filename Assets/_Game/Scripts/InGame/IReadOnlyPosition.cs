using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// Vector3で座標を読み取る用途でクラスや構造体を利用する際に使用
    /// </summary>
    public interface IReadOnlyPosition
    {
        Vector3 Position { get; }
    }
}
