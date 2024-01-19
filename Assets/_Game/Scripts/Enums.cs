using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight
    }

    public enum AudioKey
    {
        PlayerSendSE, // プレイヤーが送信した
        CharacterSendSE, // キャラクターが送信した
        TabOpenCloseSE, // チャットウィンドウの開閉
        CharacterTouchSE, // キャラクターをタッチした
    }

    public enum ParticleKey
    {
        MouseClick, // 画面をクリック
        SimpleNotice, // シンプル状態でキャラクターが喋る
    }
}
