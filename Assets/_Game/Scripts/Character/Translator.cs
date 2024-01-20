using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class Translator
    {
        /// <summary>
        /// 現在のゲームの状態からAPIにリクエストするメッセージを生成
        /// </summary>
        public static string Translate(IReadOnlyGameState gameState)
        {
            string request = string.Empty;
            if (gameState.OnStageBorder) request += "これ以上進むとステージから落ちてしまうかもしれません。";
            if (gameState.OnHoleFront) request += "目の前にはジャンプで飛び越えられる穴があります。";
            if (gameState.OnStepFront) request += "目の前にはジャンプで飛び越えられる段差があります。";
            if (!(gameState.OnStageBorder ||
                  gameState.OnHoleFront ||
                  gameState.OnStepFront)) request += "直進できる道が続いています。";

            request += "あなたはどの方向に進みますか？";

            return request;
        }

        /// <summary>
        /// プレイヤーが送信した内容からAPIにリクエストするメッセージを生成
        /// </summary>
        public static string Translate(string playerSend)
        {
            string request = "次の指示に従ってください。" + playerSend;
            return request;
        }
    }
}