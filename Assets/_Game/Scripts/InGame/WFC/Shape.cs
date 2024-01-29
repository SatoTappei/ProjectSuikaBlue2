using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile = PSB.Game.WFC.Cell.Tile;

namespace PSB.Game.WFC
{
    public static class Shape
    {
        // 床:0 壁:1
        // 上下左右 の順番

        public static readonly int[][] Floor =
        {
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
        };

        public static readonly int[][] Cross =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
        };

        public static readonly int[][] Vertical =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
        };

        public static readonly int[][] Horizontal =
        {
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
        };

        public static readonly int[][] UpLeft =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
        };

        public static readonly int[][] UpRight =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
        };

        public static readonly int[][] DownRight =
        {
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
        };

        public static readonly int[][] DownLeft =
        {
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
        };

        public static readonly int[][] UpT =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
        };

        public static readonly int[][] DownT =
        {
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
        };

        public static readonly int[][] LeftT =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
        };

        public static readonly int[][] RightT =
        {
            new int[]{ 0,1,0 },
            new int[]{ 0,1,0 },
            new int[]{ 0,0,0 },
            new int[]{ 0,1,0 },
        };

        public static int[][] Get(Tile type)
        {
            if (type == Tile.Floor) return Floor;
            if (type == Tile.Cross) return Cross;
            if (type == Tile.DownLeft) return DownLeft;
            if (type == Tile.UpLeft) return UpLeft;
            if (type == Tile.DownRight) return DownRight;
            if (type == Tile.UpRight) return UpRight;
            if (type == Tile.Vertical) return Vertical;
            if (type == Tile.Horizontal) return Horizontal;
            if (type == Tile.UpT) return UpT;
            if (type == Tile.DownT) return DownT;
            if (type == Tile.LeftT) return LeftT;
            if (type == Tile.RightT) return RightT;

            throw new System.ArgumentException("タイルに対応する形状が無い " + type);
        }
    }
}
