using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
        public void Location(LocationKey key, Vector3 position)
        {
            if(_locations.ContainsKey(key))
            {
                Instantiate(_locations[key], position, Quaternion.identity);
            }
        }

        /// <summary>
        /// �A�C�e����ݒu
        /// </summary>
        public void Item(ItemKey key, Vector3 position)
        {
            if (_items.ContainsKey(key))
            {
                Instantiate(_items[key], position, Quaternion.identity);
            }
        }

        /// <summary>
        /// �L�����N�^�[��ݒu
        /// </summary>
        public void Character(CharacterKey key, Vector3 position)
        {
            if (_characters.ContainsKey(key))
            {
                Instantiate(_characters[key], position, Quaternion.identity);
            }
        }
    }
}
