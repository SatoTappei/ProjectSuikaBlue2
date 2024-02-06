using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    // 複数のクラスで使用される列挙型はここに記述する。
    // 一括で管理するため全シーン共通で使う。

    /// <summary>
    /// キャラクターの向いている方向を表す。
    /// Z軸の正の方向を北、X軸の正の方向を東とする。
    /// </summary>
    public enum Direction
    {
        East,
        West,
        North,
        South,
    }

    /// <summary>
    /// タイルがどのような場所なのかを決めるキー
    /// </summary>
    public enum LocationKey
    {
        Normal,
        Start,
        Goal,
    }

    /// <summary>
    /// アイテムを判定するためのキー
    /// </summary>
    public enum ItemKey
    {
        Dummy,
    }

    /// <summary>
    /// キャラクターを判定するためのキー
    /// </summary>
    public enum CharacterKey
    {
        Dummy,
    }

    /// <summary>
    /// 再生する音を指定するためのキー
    /// </summary>
    public enum AudioKey
    {
        PlayerSendSE, // プレイヤーが送信した
        CharacterSendSE, // キャラクターが送信した
        TabOpenCloseSE, // チャットウィンドウの開閉
        CharacterTouchSE, // キャラクターをタッチした
        WalkStepSE,
    }

    /// <summary>
    /// 再生するパーティクルを指定するためのキー
    /// </summary>
    public enum ParticleKey
    {
        MouseClick, // 画面をクリック
        SimpleNotice, // シンプル状態でキャラクターが喋る
    }
}
