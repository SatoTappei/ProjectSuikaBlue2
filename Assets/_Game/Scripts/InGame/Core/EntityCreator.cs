using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.Game
{
    public class EntityCreator : MonoBehaviour
    {
        [System.Serializable]
        class Data<T>
        {
            public T Key;
            public GameObject Prefab;
        }

        [Header("�ꏊ�f�[�^")]
        [SerializeField] Data<LocationKey>[] _locationData;
        [Header("�A�C�e���f�[�^")]
        [SerializeField] Data<ItemKey>[] _itemData;
        [Header("�L�����N�^�[�f�[�^")]
        [SerializeField] Data<CharacterKey>[] _characterData;

        Dictionary<LocationKey, GameObject> _locations = new();
        Dictionary<ItemKey, GameObject> _items = new();
        Dictionary<CharacterKey, GameObject> _characters = new();

        void Awake()
        {
            _locations = _locationData.ToDictionary(d => d.Key, d => d.Prefab);
            _items = _itemData.ToDictionary(d => d.Key, d => d.Prefab);
            _characters = _characterData.ToDictionary(d => d.Key, d => d.Prefab);
        }

        /// <summary>
        /// �ꏊ��ݒu
        /// </summary>
        public ILocation Location(LocationKey key, Vector3 position)
        {
            return Entity<LocationKey, ILocation>(key, position, _locations);
        }

        /// <summary>
        /// �A�C�e����ݒu
        /// </summary>
        public IItem Item(ItemKey key, Vector3 position)
        {
            return Entity<ItemKey, IItem>(key, position, _items);
        }

        /// <summary>
        /// �L�����N�^�[��ݒu
        /// </summary>
        public ICharacter Character(CharacterKey key, Vector3 position)
        {
            return Entity<CharacterKey, ICharacter>(key, position, _characters);
        }

        TEntity Entity<TKey, TEntity>(TKey k, Vector3 p, IReadOnlyDictionary<TKey, GameObject> d)
        {
            if (d.ContainsKey(k))
            {
                GameObject g = Instantiate(d[k], p, Quaternion.identity);
                return g.GetComponent<TEntity>();
            }
            else throw new KeyNotFoundException($"{d}�ɓo�^����Ă��Ȃ�: " + k);
        }
    }
}
