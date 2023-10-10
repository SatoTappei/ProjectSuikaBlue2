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

        void CreateMap(int height, int width)
        {
            _map = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    // �S�ẴZ���͂ǂ̃^�C���ɂ��Ȃ�\��������
                    TileType[] allTiles = { TileType.DownLeft, TileType.UpRight, TileType.DownRight, TileType.UpLeft };
                    _map[i, k] = new(new Vector2Int(k, i), allTiles);
                }
            }
        }

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
            temp = temp.OrderBy(c => c.SelectableTiles.Length).ToList();
            int length = temp[0].SelectableTiles.Length;
            temp = temp.Where(c => c.SelectableTiles.Length == length).ToList();
            // �Z���������_���ɑI�сA���̃Z���̃^�C���I�����̒����烉���_����1�̃^�C����I��ŕ��󂳂���
            int cellIndex = _random.NextInt(0, temp.Count);
            TileType tile = temp[cellIndex].SelectableTiles[_random.NextInt(0, temp[cellIndex].SelectableTiles.Length)];
            temp[cellIndex].Collapse(tile);
        }

        void PropagateToCell(int y, int x)
        {
            // �S�Ẵ^�C����I�����Ɋ܂߂�
            List<TileType> selectableTiles = new()
            {
                TileType.Floor, TileType.Cross, TileType.Vertical, TileType.Horizontal,
                TileType.DownLeft, TileType.UpRight, TileType.DownRight, TileType.UpLeft,
                TileType.UpT, TileType.DownT, TileType.LeftT, TileType.RightT,
            };
            // �Ώۂ̃Z���̏㉺���E�̃Z���𒲂ׂ�
            foreach (Vector2Int dir in FourDirections)
            {
                int dy = y + dir.y;
                int dx = x + dir.x;
                // �}�b�v�͈͓̔����`�F�b�N
                if (!IsWithinLength(dy, dx)) continue;
                Vector2Int reverse = ReverseDirection(dir);
                // �w������̃Z���̑I���\�ȃ^�C���Ɋ�Â��āA�ڑ��\�ȃ^�C����validTiles�ɒǉ�
                List<TileType> validTiles = new();
                foreach (TileType tile in _map[dy, dx].SelectableTiles)
                {
                    IReadOnlyList<TileType> tilesForDirection = _rule.GetConnectableTilesForDirection(tile, reverse);
                    validTiles = validTiles.Concat(tilesForDirection).ToList();
                }
                // �ϏW�������A�L���ȃ^�C���ȊO���Ȃ�
                selectableTiles = selectableTiles.Intersect(validTiles).ToList();
            }
            // �Z�����I���ł���^�C���Ƃ��Ĕ��f����
            _map[y, x].SelectableTiles = selectableTiles.ToArray();
        }

        Vector2Int ReverseDirection(Vector2Int dir)
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
    }
}
