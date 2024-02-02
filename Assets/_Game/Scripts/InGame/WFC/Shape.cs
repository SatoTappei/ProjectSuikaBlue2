using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile = PSB.Game.WFC.Cell.Tile;

namespace PSB.Game.WFC
{
    public static class Shape
    {
        // ���]�������ۂɕ��т�����Ȃ��悤���ォ�玞�v���ɔԍ������蓖�Ă�B
        // ��(�����E), �E(�㒆��), ��(�E����), ��(������)
        // ��]����ꍇ�����v���B

        public const int Up = 0, Right = 1, Down = 2, Left = 3;

        // ��                            
        static readonly string[] Floor = { "000", "000", "000", "000", };
        // �\��
        static readonly string[] Cross = { "010", "010", "010", "010" };
        // �p
        static readonly string[] Corner = { "010", "010", "000", "000" };
        // ����
        static readonly string[] Straight = { "010", "000", "010", "000" };
        // T��
        static readonly string[] TJi = { "010", "010", "000", "010" };

        /// <summary>
        /// ��ƂȂ�`���K�X��]���ĕԂ��B
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

            throw new System.ArgumentException("�^�C���ɑΉ�����`�󂪖��� " + type);
        }

        // ���v����90�x���݂ŉ�]�����ďo����4�ӂ̌`��̃p�^�[��
        static string[][] Pattern(string[] a)
        {
            string[][] p =
            {
                new string[]{ a[Up], a[Right], a[Down], a[Left] }, // ��]���Ă��Ȃ����
                new string[]{ a[Left], a[Up], a[Right], a[Down] }, // ���v����90�x��]�������
                new string[]{ a[Down], a[Left], a[Up], a[Right] }, // ���v����180�x��]�������
                new string[]{ a[Right], a[Down], a[Left], a[Up] }, // ���v����270�x��]�������
            };

            return p;
        }
    }
}
