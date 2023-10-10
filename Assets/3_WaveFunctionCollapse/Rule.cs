using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace PSB.WaveFunctionCollapse
{
    public class Rule
    {
        List<TileType>[] _floor = new List<TileType>[4];
        List<TileType>[] _cross = new List<TileType>[4];
        List<TileType>[] _vertical = new List<TileType>[4];
        List<TileType>[] _horizontal = new List<TileType>[4];
        List<TileType>[] _upLeft = new List<TileType>[4];
        List<TileType>[] _upRight = new List<TileType>[4];
        List<TileType>[] _downRight = new List<TileType>[4];
        List<TileType>[] _downLeft = new List<TileType>[4];
        List<TileType>[] _upT = new List<TileType>[4];
        List<TileType>[] _downT = new List<TileType>[4];
        List<TileType>[] _leftT = new List<TileType>[4];
        List<TileType>[] _rightT = new List<TileType>[4];

        public Rule()
        {
            Create();
        }

        void Create()
        {
            for (int i = 0; i < Enum.GetValues(typeof(TileType)).Length; i++)
            {
                TileType tileA = (TileType)i;

                List<TileType>[] rule = Dictionary(tileA);
                for (int k = 0; k < 4; k++) rule[k] = new();

                int[][] shapeA = Shape.Get(tileA);
                for (int k = 0; k < Enum.GetValues(typeof(TileType)).Length; k++)
                {
                    TileType tileB = (TileType)k;
                    int[][] shapeB = Shape.Get(tileB);

                    // タイルAの方向の形状と、タイルBの反対側の方向の形状が同じ場合
                    // 接続可能な形状としてタイルAのリストに追加する
                    if (shapeA[0].SequenceEqual(shapeB[1])) rule[0].Add(tileB);
                    if (shapeA[1].SequenceEqual(shapeB[0])) rule[1].Add(tileB);
                    if (shapeA[2].SequenceEqual(shapeB[3])) rule[2].Add(tileB);
                    if (shapeA[3].SequenceEqual(shapeB[2])) rule[3].Add(tileB);
                }
            }
        }

        List<TileType>[] Dictionary(TileType tile)
        {
            if (tile == TileType.Floor) return _floor;
            if (tile == TileType.Cross) return _cross;
            if (tile == TileType.DownLeft) return _downLeft;
            if (tile == TileType.UpLeft) return _upLeft;
            if (tile == TileType.DownRight) return _downRight;
            if (tile == TileType.UpRight) return _upRight;
            if (tile == TileType.Vertical) return _vertical;
            if (tile == TileType.Horizontal) return _horizontal;
            if (tile == TileType.UpT) return _upT;
            if (tile == TileType.DownT) return _downT;
            if (tile == TileType.LeftT) return _leftT;
            if (tile == TileType.RightT) return _rightT;
            else throw new System.ArgumentException("タイルに対応する形状が無い " + tile);
        }

        public IReadOnlyList<TileType> GetConnectableTilesForDirection(TileType tile, Vector2Int dir)
        {
            List<TileType>[] connectableTiles = Dictionary(tile);

            if (dir == Vector2Int.up) return connectableTiles[0];
            if (dir == Vector2Int.down) return connectableTiles[1];
            if (dir == Vector2Int.left) return connectableTiles[2];
            if (dir == Vector2Int.right) return connectableTiles[3];
            throw new System.ArgumentException("上下左右以外のVector2Intが引数になっている: " + dir);
        }
    }
}
