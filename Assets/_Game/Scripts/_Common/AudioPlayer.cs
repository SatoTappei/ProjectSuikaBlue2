using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.SceneManagement;

namespace PSB.Game
{
    /// <summary>
    /// 自身へのメッセージングとstaticなフラグでインスタンスの有無を管理することにより
    /// 複数シーンを読み込んで使う場合にシングルトンのような振る舞いをする。
    /// </summary>
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

        // シングルトン的な使い方するためのフラグ
        static bool _first = true;

        [System.Serializable]
        class Data
        {
            public AudioKey Key;
            public AudioClip Clip;
            [Range(0, 1)]
            public float Volume = 1;
        }

        [SerializeField] Data[] _data;
        [Header("複数シーンで再生した際に一緒に削除する")]
        [SerializeField] AudioListener _audioListener;

        Dictionary<AudioKey, Data> _table;
        AudioSource[] _sources = new AudioSource[Max];

        void Awake()
        {
            // シングルトン風なチェック
            if (_first) _first = false;
            else
            {
                // 警告が出るのでListenerもついでに無効化しておく
                if (_audioListener != null) _audioListener.enabled = false;
                gameObject.SetActive(false);
                return;
            }

            // AudioSourceをたくさん追加
            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
            }

            // 音データを辞書に追加
            _table = _data.ToDictionary(v => v.Key, v => v);

            // 自身に送信したメッセージを受信して再生/停止
            MessageBroker.Default.Receive<PlayMessage>()
                .Where(_ => gameObject.activeSelf).Subscribe(Play).AddTo(this);
            MessageBroker.Default.Receive<StopMessage>()
                .Where(_ => gameObject.activeSelf).Subscribe(StopBGM).AddTo(this);
        }

        void OnDestroy()
        {
            _first = true;
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
            MessageBroker.Default.Publish(new PlayMessage() 
            {
                Key = key, 
                Mode = mode,
            });
        }

        /// <summary>
        /// 繰り返し音を再生
        /// </summary>
        public static void PlayLoop(MonoBehaviour mono, int count, float delay, AudioKey key, PlayMode mode)
        {
            mono.StartCoroutine(M());

            IEnumerator M()
            {
                for(int i = 0; i < count; i++)
                {
                    Play(key, mode);
                    yield return new WaitForSeconds(delay);
                }
            }
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