using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// ダンジョン自体を形成するオブジェクトに実装し、任意の修正を行う
    /// </summary>
    public interface IDungeonParts
    {
        public void Remove();
    }
}