using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Tile = PSB.Game.WFC.Cell.Tile;

namespace PSB.Game.WFC
{
    public class Rule
    {
        List<Tile>[] _floor = new List<Tile>[4];
        List<Tile>[] _cross = new List<Tile>[4];
        List<Tile>[] _vertical = new List<Tile>[4];
        List<Tile>[] _horizontal = new List<Tile>[4];
        List<Tile>[] _upLeft = new List<Tile>[4];
        List<Tile>[] _upRight = new List<Tile>[4];
        List<Tile>[] _downRight = new List<Tile>[4];
        List<Tile>[] _downLeft = new List<Tile>[4];
        List<Tile>[] _upT = new List<Tile>[4];
        List<Tile>[] _downT = new List<Tile>[4];
        List<Tile>[] _leftT = new List<Tile>[4];
        List<Tile>[] _rightT = new List<Tile>[4];

        public Rule()
        {
            Create();
        }

        void Create()
        {
            for (int i = 0; i < Enum.GetValues(typeof(Tile)).Length; i++)
            {
                Tile tileA = (Tile)i;

                List<Tile>[] rule = Dictionary(tileA);
                for (int k = 0; k < 4; k++) rule[k] = new();

                int[][] shapeA = Shape.Get(tileA);
                for (int k = 0; k < Enum.GetValues(typeof(Tile)).Length; k++)
                {
                    Tile tileB = (Tile)k;
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

        List<Tile>[] Dictionary(Tile tile)
        {
            if (tile == Tile.Floor) return _floor;
            if (tile == Tile.Cross) return _cross;
            if (tile == Tile.DownLeft) return _downLeft;
            if (tile == Tile.UpLeft) return _upLeft;
            if (tile == Tile.DownRight) return _downRight;
            if (tile == Tile.UpRight) return _upRight;
            if (tile == Tile.Vertical) return _vertical;
            if (tile == Tile.Horizontal) return _horizontal;
            if (tile == Tile.UpT) return _upT;
            if (tile == Tile.DownT) return _downT;
            if (tile == Tile.LeftT) return _leftT;
            if (tile == Tile.RightT) return _rightT;
            else throw new System.ArgumentException("タイルに対応する形状が無い " + tile);
        }

        public IReadOnlyList<Tile> GetConnectableTilesForDirection(Tile tile, Vector2Int dir)
        {
            List<Tile>[] connectableTiles = Dictionary(tile);

            if (dir == Vector2Int.up) return connectableTiles[0];
            if (dir == Vector2Int.down) return connectableTiles[1];
            if (dir == Vector2Int.left) return connectableTiles[2];
            if (dir == Vector2Int.right) return connectableTiles[3];
            throw new System.ArgumentException("上下左右以外のVector2Intが引数になっている: " + dir);
        }
    }
}
