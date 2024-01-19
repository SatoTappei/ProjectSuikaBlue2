using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings_")]
public class CharacterSettings : ScriptableObject
{
    public readonly int MaxMental = 100;
    public readonly int MinMental = 0;

    [SerializeField] string _characterName = "めいど";
    [SerializeField] int _defaultMental = 100;

    /// <summary>
    /// 会話履歴用のヘッダー
    /// </summary>
    public string LogHeader => $"{_characterName}: ";
    /// <summary>
    /// 心情の初期値
    /// </summary>
    public int DefaultMental => _defaultMental;
}
