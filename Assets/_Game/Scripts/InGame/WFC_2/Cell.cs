using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.Game.WFC2
{
    public class Cell
    {
        public enum Tile
        {
            Floor,
            // 柱
            PillarLD,
            PillarLU,
            PillarRD,
            PillarRU,
            // 壁
            WallU,
            WallD,
            WallL,
            WallR,
            // 通路
            PathUD,
            PathLR,
        }

        public Vector2Int Index;
        public bool IsCollapsed;
        public Tile[] SelectableTiles;

        public Cell(Vector2Int index, params Tile[] tiles)
        {
            Index = index;
            SelectableTiles = tiles;
        }

        public Tile SelectedTile => SelectableTiles[0];

        public bool Collapse(Tile tile)
        {
            // 未崩壊かつ、選択肢に含まれているタイルの場合は、そのタイルに崩壊させる
            if (!IsCollapsed && SelectableTiles.Contains(tile))
            {
                IsCollapsed = true;
                SelectableTiles = new Tile[1] { tile };
                return true;
            }
            else
            {
                Debug.LogWarning($"セルの崩壊に失敗 崩壊済み:{IsCollapsed} 選んだタイル:{tile}");
                return false;
            }
        }
    }
}
