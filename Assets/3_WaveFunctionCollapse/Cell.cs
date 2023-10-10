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
            // �����󂩂A�I�����Ɋ܂܂�Ă���^�C���̏ꍇ�́A���̃^�C���ɕ��󂳂���
            if (!IsCollapsed && SelectableTiles.Contains(tile))
            {
                IsCollapsed = true;
                SelectableTiles = new TileType[1] { tile };
                return true;
            }
            else
            {
                Debug.LogWarning($"�Z���̕���Ɏ��s ����ς�:{IsCollapsed} �I�񂾃^�C��:{tile}");
                return false;
            }
        }
    }
}