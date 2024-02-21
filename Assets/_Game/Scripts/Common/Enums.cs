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
    /// 上下左右を指定する際に使用する。
    /// </summary>
    public enum Arrow
    {
        Up,
        Down,
        Left,
        Right,
    }

    /// <summary>
    /// タイルがどのような場所なのかを決めるキー
    /// </summary>
    public enum LocationKey
    {
        Normal,
        Start,
        Chest,
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
        Player,
        Enemy,
        Turret,
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
        WalkStepSE, // 歩く
        DoorOpenSE, // ドア開く
        DoorCloseSE, // ドア閉じる
        KickDamageSE, // 蹴りを喰らった
        TurretFireSE, // タレットが射撃
        GameBGM, // ゲームを通して流れる
        TreasureSE, // 宝箱獲得
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