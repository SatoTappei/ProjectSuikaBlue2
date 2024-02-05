using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// 読み取り専用のセル
    /// </summary>
    public interface IReadOnlyCell
    {
        public Vector2Int Index { get; }
        public Vector3 Position { get; }
        public LocationKey Location { get; }
        public ItemKey Item { get; }
        public CharacterKey Character { get; }
    }
}
