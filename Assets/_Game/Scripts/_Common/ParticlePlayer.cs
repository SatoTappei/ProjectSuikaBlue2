using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using UniRx.Toolkit;

namespace PSB.Game
{
    public class ParticlePlayer : MonoBehaviour
    {
        #region ���g�Ƀ��b�Z�[�W���O����p
        struct Message
        {
            public ParticleKey Key;
            public Vector3 Position;
            public Transform Parent;
        }
        #endregion
        #region �p�[�e�B�N���̃v�[��
        class Pool : ObjectPool<ParticleSystem>
        {
            ParticleSystem _original;
            Transform _parent;

            public Pool(ParticleSystem original, Transform parent)
            {
                _original = original;
                _parent = parent;
            }

            protected override ParticleSystem CreateInstance()
            {
                ParticleSystem p = Instantiate(_original, _parent);
                p.transform.position = Vector3.zero;
                return p;
            }
        }
        #endregion

        [System.Serializable]
        class Data
        {
            public ParticleKey Key;
            public ParticleSystem Prefab;
            [Tooltip("�v�[�����Ă�����")]
            public int Pooled;
        }

        [SerializeField] Data[] _data;

        Transform _pool;
        Dictionary<ParticleKey, Pool> _table;

        void Awake()
        {
            _pool = new GameObject("ParticlePool").transform;
            _table = _data.ToDictionary(d => d.Key, d => new Pool(d.Prefab, _pool));

            MessageBroker.Default.Receive<Message>().Subscribe(Play).AddTo(this);
        }

        // �w�肵���ʒu����e��Ǐ]����悤�ȃp�[�e�B�N�����Đ�
        void Play(Message msg)
        {
            if (_table.TryGetValue(msg.Key, out Pool pool))
            {
                ParticleSystem p = pool.Rent();
                p.transform.position = msg.Position;
                p.Play();
            }
            else
            {
                Debug.LogError("�Ή�����p�[�e�B�N�����Ȃ�: " + msg.Key);
            }
        }

        /// <summary>
        /// �Đ�
        /// </summary>
        public void Play(ParticleKey key, Vector3 position, Transform parent = null)
        {
            MessageBroker.Default.Publish(new Message() { Key = key, Parent = parent });
        }
    }
}