using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Tile = PSB.Game.WFC2.Cell.Tile;

namespace PSB.Game.WFC2
{
    public class Rule
    {
        List<Tile>[] _floor = new List<Tile>[4];
        List<Tile>[] _pillarLD = new List<Tile>[4];
        List<Tile>[] _pillarLU = new List<Tile>[4];
        List<Tile>[] _pillarRD = new List<Tile>[4];
        List<Tile>[] _pillarRU = new List<Tile>[4];
        List<Tile>[] _wallU = new List<Tile>[4];
        List<Tile>[] _wallD = new List<Tile>[4];
        List<Tile>[] _wallL = new List<Tile>[4];
        List<Tile>[] _wallR = new List<Tile>[4];
        List<Tile>[] _pathUD = new List<Tile>[4];
        List<Tile>[] _pathLR = new List<Tile>[4];

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
            if (tile == Tile.PillarLD) return _pillarLD;
            if (tile == Tile.PillarLU) return _pillarLU;
            if (tile == Tile.PillarRD) return _pillarRD;
            if (tile == Tile.PillarRU) return _pillarRU;
            if (tile == Tile.WallD) return _wallD;
            if (tile == Tile.WallL) return _wallL;
            if (tile == Tile.WallR) return _wallR;
            if (tile == Tile.WallU) return _wallU;
            if (tile == Tile.PathLR) return _pathLR;
            if (tile == Tile.PathUD) return _pathUD;
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
