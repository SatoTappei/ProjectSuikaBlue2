using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile = PSB.Game.WFC.Cell.Tile;

namespace PSB.Game.WFC
{
    public static class Shape
    {
        // 反転させた際に並びが崩れないよう左上から時計回りに番号を割り当てる。
        // 上(左中右), 右(上中下), 下(右中左), 左(下中上)
        // 回転する場合も時計回り。

        public const int Up = 0, Right = 1, Down = 2, Left = 3;

        // 床                            
        static readonly string[] Floor = { "000", "000", "000", "000", };
        // 十字
        static readonly string[] Cross = { "010", "010", "010", "010" };
        // 角
        static readonly string[] Corner = { "010", "010", "000", "000" };
        // 直線
        static readonly string[] Straight = { "010", "000", "010", "000" };
        // T字
        static readonly string[] TJi = { "010", "010", "000", "010" };

        /// <summary>
        /// 基となる形状を適宜回転して返す。
        /// </summary>
        public static string[] GetQuad(Tile type)
        {
            if (type == Tile.Floor) return Floor;

            if (type == Tile.Cross) return Cross;

            if (type == Tile.CornerU) return Pattern(Corner)[Up];
            if (type == Tile.CornerR) return Pattern(Corner)[Right];
            if (type == Tile.CornerD) return Pattern(Corner)[Down];
            if (type == Tile.CornerL) return Pattern(Corner)[Left];

            if (type == Tile.StraightU) return Pattern(Straight)[Up];
            if (type == Tile.StraightR) return Pattern(Straight)[Right];

            if (type == Tile.TJiU) return Pattern(TJi)[Up];
            if (type == Tile.TJiR) return Pattern(TJi)[Right];
            if (type == Tile.TJiD) return Pattern(TJi)[Down];
            if (type == Tile.TJiL) return Pattern(TJi)[Left];

            throw new System.ArgumentException("タイルに対応する形状が無い " + type);
        }

        // 時計回りに90度刻みで回転させて出来る4辺の形状のパターン
        static string[][] Pattern(string[] a)
        {
            string[][] p =
            {
                new string[]{ a[Up], a[Right], a[Down], a[Left] }, // 回転していない状態
                new string[]{ a[Left], a[Up], a[Right], a[Down] }, // 時計回りに90度回転した状態
                new string[]{ a[Down], a[Left], a[Up], a[Right] }, // 時計回りに180度回転した状態
                new string[]{ a[Right], a[Down], a[Left], a[Up] }, // 時計回りに270度回転した状態
            };

            return p;
        }
    }
}
