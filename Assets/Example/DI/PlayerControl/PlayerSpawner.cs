using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PSB.DI.PlayerControl
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _prefab;

        GameObject _player;
        IObjectResolver _resolver;

        [Inject]
        void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        void Start()
        {
            InstantiatePlayer();
        }

        void InstantiatePlayer()
        {
            if(_player != null)
            {
                Debug.LogWarning("Šù‚ÉƒvƒŒƒCƒ„[‚ğ¶¬Ï‚İ");
                return;
            }

            _player = _resolver.Instantiate(_prefab, Vector3.up, Quaternion.identity);
        }
    }
}
