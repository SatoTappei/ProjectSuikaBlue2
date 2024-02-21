using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    [CreateAssetMenu(fileName = "CharacterSettings_")]
    public class CharacterSettings : ScriptableObject
    {
        [Header("キャラクターの名前")]
        [SerializeField] string _characterName = "めいど";
        [Header("心情")]
        [SerializeField] int _maxMental = 100;
        [SerializeField] int _minMental = 0;
        [SerializeField] int _defaultMental = 100;
        [Header("プレイヤーに返答の優先度")]
        [SerializeField] int _playerPriority = 100;
        [Header("各種イベントにリアクションの優先度")]
        [SerializeField] int _inGameEventPriority = 1000;
        [Header("被ダメージにリアクションの優先度")]
        [SerializeField] int _damagedPriority = 500;

        /// <summary>
        /// 会話履歴用のヘッダー
        /// </summary>
        public string LogHeader => $"{_characterName}: ";
        /// <summary>
        /// 心情の最大値
        /// </summary>
        public int MaxMental => _maxMental;
        /// <summary>
        /// 心情の最小値
        /// </summary>
        public int MinMental => _minMental;
        /// <summary>
        /// 心情の初期値
        /// </summary>
        public int DefaultMental => Mathf.Clamp(_defaultMental, _minMental, _maxMental);
        /// <summary>
        /// プレイヤーに返答の優先度
        /// </summary>
        public int PlayerPriority => _playerPriority;
        /// <summary>
        /// 各種イベントにリアクションの優先度
        /// </summary>
        public int InGameEventPriority => _inGameEventPriority;
        /// <summary>
        /// ダメージを受けた際にリアクションの優先度
        /// </summary>
        public int DamagedPriority => _damagedPriority;
    }
}
