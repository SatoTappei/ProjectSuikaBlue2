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
        // 全てのタイルの4辺それぞれに対して接続可能なタイルを保持
        Dictionary<Tile, List<Tile>[]> _all = new();

        public Rule()
        {
            foreach (Tile tileA in Enum.GetValues(typeof(Tile)))
            {
                // タイルをキーにした値を初期化
                _all.Add(tileA, new List<Tile>[4]); // 4辺保持するので長さは4固定
                for (int k = 0; k < 4; k++) _all[tileA][k] = new();

                // 2つのタイルの形状の組み合わせを全て調べる
                string[] shapeA = Shape.GetQuad(tileA);
                foreach (Tile tileB in Enum.GetValues(typeof(Tile)))
                {
                    string[] shapeB = Shape.GetQuad(tileB);

                    // タイルAの方向の形状と、タイルBの反対側の方向の形状が同じ場合
                    // 接続可能な形状としてタイルAのリストに追加する
                    if (shapeA[Shape.Up].SequenceEqual(shapeB[Shape.Down].Reverse())) _all[tileA][Shape.Up].Add(tileB);
                    if (shapeA[Shape.Right].SequenceEqual(shapeB[Shape.Left].Reverse())) _all[tileA][Shape.Right].Add(tileB);
                    if (shapeA[Shape.Down].SequenceEqual(shapeB[Shape.Up].Reverse())) _all[tileA][Shape.Down].Add(tileB);
                    if (shapeA[Shape.Left].SequenceEqual(shapeB[Shape.Right].Reverse())) _all[tileA][Shape.Left].Add(tileB);
                }
            }
        }

        /// <summary>
        /// その方向の辺が接続可能なタイルを返す。
        /// </summary>
        public IReadOnlyList<Tile> GetConnectableTiles(Tile tile, Vector2Int dir)
        {
            if (dir == Vector2Int.up) return _all[tile][Shape.Up];
            if (dir == Vector2Int.right) return _all[tile][Shape.Right];
            if (dir == Vector2Int.down) return _all[tile][Shape.Down];
            if (dir == Vector2Int.left) return _all[tile][Shape.Left];

            throw new System.ArgumentException("上下左右以外のVector2Intが引数になっている: " + dir);
        }
    }
}
