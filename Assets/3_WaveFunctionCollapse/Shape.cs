using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.WaveFunctionCollapse
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

        public static int[][] Get(TileType type)
        {
            if (type == TileType.Floor) return Floor;
            if (type == TileType.Cross) return Cross;
            if (type == TileType.DownLeft) return DownLeft;
            if (type == TileType.UpLeft) return UpLeft;
            if (type == TileType.DownRight) return DownRight;
            if (type == TileType.UpRight) return UpRight;
            if (type == TileType.Vertical) return Vertical;
            if (type == TileType.Horizontal) return Horizontal;
            if (type == TileType.UpT) return UpT;
            if (type == TileType.DownT) return DownT;
            if (type == TileType.LeftT) return LeftT;
            if (type == TileType.RightT) return RightT;
            
            throw new System.ArgumentException("タイルに対応する形状が無い " + type);
        }
    }
}
