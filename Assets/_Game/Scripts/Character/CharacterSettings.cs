using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "CharacterSettings_")]
    public class CharacterSettings : ScriptableObject
    {
        public readonly int MaxMental = 100;
        public readonly int MinMental = 0;

        [Header("キャラクターの名前")]
        [SerializeField] string _characterName = "めいど";
        [Header("デフォルトの心情")]
        [SerializeField] int _defaultMental = 100;
        [Header("プレイヤーの優先度")]
        [SerializeField] int _playerPriority = 100;

        /// <summary>
        /// 会話履歴用のヘッダー
        /// </summary>
        public string LogHeader => $"{_characterName}: ";
        /// <summary>
        /// 心情の初期値
        /// </summary>
        public int DefaultMental => _defaultMental;
        /// <summary>
        /// プレイヤーの優先度
        /// </summary>
        public int PlayerPriority => _playerPriority;
    }
}
