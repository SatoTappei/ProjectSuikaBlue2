using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile = PSB.Game.WFC2.Cell.Tile;

namespace PSB.Game.WFC2
{
    public static class Shape
    {
        // ���ォ�玞�v���ɔԍ������蓖�Ă�
        // ��]����ꍇ�����v���

        static readonly int[][] Floor =
        {
            new int[]{ 0,0,0 }, // �� { ��,��,�E }
            new int[]{ 0,0,0 }, // �E { ��,��,�� }
            new int[]{ 0,0,0 }, // �� { �E,��,�� }
            new int[]{ 0,0,0 }, // �� { ��,��,�� }
        };

        static readonly int[][] Pillar =
        {
            new int[]{ 1,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,1 },
        };

        static readonly int[][] Wall =
        {
            new int[]{ 1,1,1 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
        };

        static readonly int[][] Path =
        {
            new int[]{ 1,1,1 },
            new int[]{ 0,0,0 },
            new int[]{ 1,1,1 },
            new int[]{ 0,0,0 },
        };

        public static int[][] Get(Tile type)
        {
            if (type == Tile.Floor) return Floor;
            if (type == Tile.PillarLU) return Pattern(Pillar)[0];
            if (type == Tile.PillarRU) return Pattern(Pillar)[1];
            if (type == Tile.PillarRD) return Pattern(Pillar)[2];
            if (type == Tile.PillarLD) return Pattern(Pillar)[3];
            if (type == Tile.WallU) return Pattern(Wall)[0];
            if (type == Tile.WallR) return Pattern(Wall)[1];
            if (type == Tile.WallD) return Pattern(Wall)[2];
            if (type == Tile.WallL) return Pattern(Wall)[3];
            if (type == Tile.PathUD) return Pattern(Path)[0];
            if (type == Tile.PathLR) return Pattern(Path)[1];

            throw new System.ArgumentException("�^�C���ɑΉ�����`�󂪖��� " + type);
        }

        static int[][][] Pattern(int[][] a)
        {
            int[][][] p =
            {
                new int[][]{ a[0], a[1], a[2], a[3] }, // ��]���Ă��Ȃ����
                new int[][]{ a[3], a[0], a[1], a[2] }, // ���v����90�x��]�������
                new int[][]{ a[2], a[3], a[0], a[1] }, // ���v����180�x��]�������
                new int[][]{ a[1], a[2], a[3], a[0] }, // ���v����270�x��]�������
            };

            return p;
        }
    }
}
