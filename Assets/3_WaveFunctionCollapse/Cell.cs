using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.WaveFunctionCollapse
{
    public class Cell
    {
        public Vector2Int Index;
        public bool IsCollapsed;
        public TileType[] SelectableTiles;

        public TileType SelectedTile => SelectableTiles[0];

        public Cell(Vector2Int index, params TileType[] tiles)
        {
            Index = index;
            SelectableTiles = tiles;
        }

        public bool Collapse(TileType tile)
        {
            // 未崩壊かつ、選択肢に含まれているタイルの場合は、そのタイルに崩壊させる
            if (!IsCollapsed && SelectableTiles.Contains(tile))
            {
                IsCollapsed = true;
                SelectableTiles = new TileType[1] { tile };
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