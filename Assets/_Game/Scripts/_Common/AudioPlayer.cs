using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace PSB.Game
{
    public class AudioPlayer : MonoBehaviour
    {
        #region 自身にメッセージングする用
        public enum PlayMode
        {
            SE,
            BGM,
        }

        struct PlayMessage
        {
            public AudioKey Key;
            public PlayMode Mode;
        }

        struct StopMessage { }
        #endregion

        // 同時再生出来る最大数
        const int Max = 10;

        [System.Serializable]
        class Data
        {
            public AudioKey Key;
            public AudioClip Clip;
            [Range(0, 1)]
            public float Volume = 1;
        }

        [SerializeField] Data[] _data;

        Dictionary<AudioKey, Data> _table;
        AudioSource[] _sources = new AudioSource[Max];

        void Awake()
        {
            // AudioSourceをたくさん追加
            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
            }

            // 音データを辞書に追加
            _table = _data.ToDictionary(v => v.Key, v => v);

            // 自身に送信したメッセージを受信して再生/停止
            MessageBroker.Default.Receive<PlayMessage>().Subscribe(Play).AddTo(this);
            MessageBroker.Default.Receive<StopMessage>().Subscribe(StopBGM).AddTo(this);
        }

        void Play(PlayMessage msg)
        {
            if (msg.Mode == PlayMode.SE)
            {
                // 先頭はBGMが使用するので1つスキップする
                AudioSource source = _sources.Skip(1).Where(v => !v.isPlaying).FirstOrDefault();
                if (source == null) return;

                source.clip = _table[msg.Key].Clip;
                source.volume = _table[msg.Key].Volume;
                source.loop = false;
                source.Play();
            }
            else if (msg.Mode == PlayMode.BGM)
            {
                _sources[0].clip = _table[msg.Key].Clip;
                _sources[0].volume = _table[msg.Key].Volume;
                _sources[0].loop = true;
                _sources[0].Play();
            }
        }

        /// <summary>
        /// BGMを停止する
        /// </summary>
        void StopBGM(StopMessage msg)
        {
            _sources[0].Stop();
        }

        /// <summary>
        /// 音を再生
        /// </summary>
        public static void Play(AudioKey key, PlayMode mode)
        {
            MessageBroker.Default.Publish(new PlayMessage() { Key = key, Mode = mode });
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        public static void StopBGM()
        {
            MessageBroker.Default.Publish(new StopMessage());
        }
    }
}