using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = Unity.Mathematics.Random;

namespace PSB.WaveFunctionCollapse
{
    public class Logic
    {
        // �㉺���E
        static readonly Vector2Int[] FourDirections =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        Rule _rule;
        Cell[,] _map;
        Random _random;

        public Logic(int height, int width, uint seed)
        {
            _rule = new();
            _random = new Random(seed);
            CreateMap(height, width);
        }

        TileType[] TileTypeAll => (TileType[])System.Enum.GetValues(typeof(TileType));

        void CreateMap(int height, int width)
        {
            _map = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    // �S�ẴZ���͂ǂ̃^�C���ɂ��Ȃ�\��������
                    TileType[] allTiles = TileTypeAll;
                    _map[i, k] = new(new Vector2Int(k, i), allTiles);
                }
            }
        }

        /// <summary>
        /// 1��ĂԖ���1�Z�����󂳂���
        /// </summary>
        public Cell[,] Step()
        {
            CollapseCell();

            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int k = 0; k < _map.GetLength(1); k++)
                {
                    if (!_map[i, k].IsCollapsed)
                    {
                        PropagateToCell(i, k);
                    }
                }
            }

            return _map;
        }

        void CollapseCell()
        {
            // �ꎞ�ۑ��̃��X�g�ɖ�����̃Z����ǉ�����
            List<Cell> temp = new();
            foreach (Cell cell in _map)
            {
                if (!cell.IsCollapsed) temp.Add(cell);
            }
            // �����_���ŕ��󂷂�Z�������߂邽�߁A�G���g���s�[����ԒႢ�Z���ȊO���Ȃ�
            temp = SortLowEntropy(temp);
            // �Z���������_���ɑI�сA���̃Z���̃^�C���I�����̒����烉���_����1�̃^�C����I��ŕ��󂳂���
            int cellIndex = _random.NextInt(0, temp.Count);
            int tileTypeIndex = _random.NextInt(0, temp[cellIndex].SelectableTileCount);
            TileType tile = temp[cellIndex].SelectableTiles[tileTypeIndex];
            temp[cellIndex].Collapse(tile);
        }

        void PropagateToCell(int y, int x)
        {
            // �S�Ẵ^�C����I�����Ɋ܂߂�
            TileType[] selectableTiles = TileTypeAll;
            // �Ώۂ̃Z���̏㉺���E�̃Z���𒲂ׂ�
            foreach (Vector2Int dir in FourDirections)
            {
                int dy = y + dir.y;
                int dx = x + dir.x;

                // �}�b�v�͈͓̔����`�F�b�N
                if (!IsWithinLength(dy, dx)) continue;

                // �w������̃Z���̑I���\�ȃ^�C���Ɋ�Â��āA�ڑ��\�ȃ^�C����validTiles�ɒǉ�
                IEnumerable<TileType> validTiles = new TileType[0];
                Vector2Int connected = ConnectedDirection(dir);
                foreach (TileType tile in _map[dy, dx].SelectableTiles)
                {
                    IReadOnlyList<TileType> tilesForDirection = _rule.GetConnectableTilesForDirection(tile, connected);
                    validTiles = validTiles.Concat(tilesForDirection);
                }
                // �ϏW�������A�L���ȃ^�C���ȊO���Ȃ�
                selectableTiles = selectableTiles.Intersect(validTiles).ToArray();
            }
            // �Z�����I���ł���^�C���Ƃ��Ĕ��f����
            _map[y, x].SelectableTiles = selectableTiles;
        }

        Vector2Int ConnectedDirection(Vector2Int dir)
        {
            if (dir == Vector2Int.right) return Vector2Int.down;
            if (dir == Vector2Int.left) return Vector2Int.up;
            if (dir == Vector2Int.down) return Vector2Int.right;
            if (dir == Vector2Int.up) return Vector2Int.left;

            throw new System.ArgumentException("�㉺���E�ȊO��Vector2Int�������ɂȂ��Ă���: " + dir);
        }

        bool IsWithinLength(int y, int x)
        {
            return 0 <= y && y < _map.GetLength(0) && 0 <= x && x < _map.GetLength(1);
        }

        List<Cell> SortLowEntropy(List<Cell> cells)
        {
            cells = cells.OrderBy(c => c.SelectableTileCount).ToList();
            int min = cells[0].SelectableTileCount;
            cells = cells.Where(c => c.SelectableTileCount == min).ToList();
            return cells;
        }
    }
}
