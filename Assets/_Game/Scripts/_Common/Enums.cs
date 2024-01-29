using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    // 複数のクラスで使用される列挙型はここに記述する。

    /// <summary>
    /// Z軸の正の方向を北、X軸の正の方向を東とする。
    /// </summary>
    public enum Direction
    {
        East,
        West,
        North,
        South,
    }

    public enum EightDirection
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
