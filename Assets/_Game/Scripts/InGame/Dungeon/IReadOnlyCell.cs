using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// “Ç‚İæ‚èê—p‚ÌƒZƒ‹
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
