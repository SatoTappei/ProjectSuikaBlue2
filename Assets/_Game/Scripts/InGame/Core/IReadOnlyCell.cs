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
        public LocationKey LocationKey { get; }
        public ItemKey ItemKey { get; }
        public CharacterKey CharacterKey { get; }
        public ILocation Location { get; }
        public IItem Item { get; }
        public ICharacter Character { get; }
        public IReadOnlyList<IReadOnlyCell> Adjacent { get; }
    }
}
