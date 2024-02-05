using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    // 複数のクラスで使用されるメッセージング用の構造体はここに記述する。
    // 一括で管理するため全シーン共通で使う。

    /// <summary>
    /// OpenAIからのレスポンスをキー入力に変換してCharacterからInGameに送信される
    /// </summary>
    public struct KeyInputMessage
    {
        public bool KeyDownA;
        public bool KeyDownS;
        public bool KeyDownD;
        public bool KeyDownW;
        public bool KeyDownSpace;
    }
}
