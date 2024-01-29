using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace PSB.Game
{
    public class AudioPlayer : MonoBehaviour
    {
        #region ���g�Ƀ��b�Z�[�W���O����p
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

        // �����Đ��o����ő吔
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
            // AudioSource����������ǉ�
            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
            }

            // ���f�[�^�������ɒǉ�
            _table = _data.ToDictionary(v => v.Key, v => v);

            // ���g�ɑ��M�������b�Z�[�W����M���čĐ�/��~
            MessageBroker.Default.Receive<PlayMessage>().Subscribe(Play).AddTo(this);
            MessageBroker.Default.Receive<StopMessage>().Subscribe(StopBGM).AddTo(this);
        }

        void Play(PlayMessage msg)
        {
            if (msg.Mode == PlayMode.SE)
            {
                // �擪��BGM���g�p����̂�1�X�L�b�v����
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
        /// BGM���~����
        /// </summary>
        void StopBGM(StopMessage msg)
        {
            _sources[0].Stop();
        }

        /// <summary>
        /// �����Đ�
        /// </summary>
        public static void Play(AudioKey key, PlayMode mode)
        {
            MessageBroker.Default.Publish(new PlayMessage() { Key = key, Mode = mode });
        }

        /// <summary>
        /// BGM���~
        /// </summary>
        public static void StopBGM()
        {
            MessageBroker.Default.Publish(new StopMessage());
        }
    }
}