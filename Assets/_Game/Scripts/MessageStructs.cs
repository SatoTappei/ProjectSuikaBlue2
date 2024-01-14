using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    /// <summary>
    /// OpenAPIからのレスポンスをキー入力に変換してCharacterからInGameに送信される。
    /// </summary>
    public struct PlayerControlMessage
    {
        public List<KeyCode> InputKeys;
    }
}
