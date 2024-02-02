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
        // �S�Ẵ^�C����4�ӂ��ꂼ��ɑ΂��Đڑ��\�ȃ^�C����ێ�
        Dictionary<Tile, List<Tile>[]> _all = new();

        public Rule()
        {
            foreach (Tile tileA in Enum.GetValues(typeof(Tile)))
            {
                // �^�C�����L�[�ɂ����l��������
                _all.Add(tileA, new List<Tile>[4]); // 4�ӕێ�����̂Œ�����4�Œ�
                for (int k = 0; k < 4; k++) _all[tileA][k] = new();

                // 2�̃^�C���̌`��̑g�ݍ��킹��S�Ē��ׂ�
                string[] shapeA = Shape.GetQuad(tileA);
                foreach (Tile tileB in Enum.GetValues(typeof(Tile)))
                {
                    string[] shapeB = Shape.GetQuad(tileB);

                    // �^�C��A�̕����̌`��ƁA�^�C��B�̔��Α��̕����̌`�󂪓����ꍇ
                    // �ڑ��\�Ȍ`��Ƃ��ă^�C��A�̃��X�g�ɒǉ�����
                    if (shapeA[Shape.Up].SequenceEqual(shapeB[Shape.Down].Reverse())) _all[tileA][Shape.Up].Add(tileB);
                    if (shapeA[Shape.Right].SequenceEqual(shapeB[Shape.Left].Reverse())) _all[tileA][Shape.Right].Add(tileB);
                    if (shapeA[Shape.Down].SequenceEqual(shapeB[Shape.Up].Reverse())) _all[tileA][Shape.Down].Add(tileB);
                    if (shapeA[Shape.Left].SequenceEqual(shapeB[Shape.Right].Reverse())) _all[tileA][Shape.Left].Add(tileB);
                }
            }
        }

        /// <summary>
        /// ���̕����̕ӂ��ڑ��\�ȃ^�C����Ԃ��B
        /// </summary>
        public IReadOnlyList<Tile> GetConnectableTiles(Tile tile, Vector2Int dir)
        {
            if (dir == Vector2Int.up) return _all[tile][Shape.Up];
            if (dir == Vector2Int.right) return _all[tile][Shape.Right];
            if (dir == Vector2Int.down) return _all[tile][Shape.Down];
            if (dir == Vector2Int.left) return _all[tile][Shape.Left];

            throw new System.ArgumentException("�㉺���E�ȊO��Vector2Int�������ɂȂ��Ă���: " + dir);
        }
    }
}
