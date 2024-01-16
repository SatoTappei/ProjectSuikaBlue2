using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.Game
{
    public static class InputMessenger
    {
        /// <summary>
        /// 文字列を解析してインゲームへの入力となるメッセージを送信する
        /// </summary>
        public static void SendMessage(IReadOnlyGameState gameState, string command)
        {
            PlayerControlMessage msg = new PlayerControlMessage();
            if (command == "1") msg.KeyDownA = true;
            if (command == "2") msg.KeyDownD = true;
            if (command == "3")
            {
                if (gameState.Forward == Player.Forward.Left) msg.KeyDownA = true;
                else if (gameState.Forward == Player.Forward.Right) msg.KeyDownD = true;

                msg.KeyDownSpace = true;
            }

            MessageBroker.Default.Publish(msg);
        }
    }
}
