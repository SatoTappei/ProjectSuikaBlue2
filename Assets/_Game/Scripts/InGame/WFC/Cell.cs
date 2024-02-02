using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.Game.WFC
{
    public class Cell
    {
        public enum Tile
        {
            // ��
            Floor,
            // �\��
            Cross,
            // ����
            StraightU,
            StraightR,
            // �p
            CornerU,
            CornerR,
            CornerD,
            CornerL,
            // T��
            TJiU,
            TJiR,
            TJiD,
            TJiL,
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
            // �����󂩂A�I�����Ɋ܂܂�Ă���^�C���̏ꍇ�́A���̃^�C���ɕ��󂳂���
            if (!IsCollapsed && SelectableTiles.Contains(tile))
            {
                IsCollapsed = true;
                SelectableTiles = new Tile[1] { tile };
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
