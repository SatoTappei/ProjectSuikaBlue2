using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.BT
{
    /// <summary>
    /// ビヘイビアツリーが値を読み書きするための黒板
    /// 1つのインスタンスを共有することでキャラクター同士の連携を想定している。
    /// </summary>
    public class BlackBoard
    {
        int _id;

        Dictionary<int, Private> _all = new();

        /// <summary>
        /// キャラクター毎の黒板を生成。
        /// 生成した黒板はIDで管理されるので、外部からIDでの取得が可能。
        /// </summary>
        public Private CreatePrivate()
        {
            int id = _id++;
            Private p = new();
            _all.Add(id, p);

            return p;
        }

        /// <summary>
        /// IDで対応した黒板を取得。
        /// </summary>
        public Private GetPrivate(int id)
        {
            if (_all.ContainsKey(id)) return _all[id];
            else throw new KeyNotFoundException("IDに対応したキャラクター毎の黒板が無い: " + id);
        }

        // 個別の値を書き込む用のクラス
        public class Private
        {
            public IReadOnlyList<IReadOnlyCell> Path { get; set; }
        }
    }
}