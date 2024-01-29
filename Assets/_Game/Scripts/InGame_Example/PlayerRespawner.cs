using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PSB.GameExample
{
    public class PlayerRespawner : MonoBehaviour
    {
        [SerializeField] Collider _penaltyArea;
        [SerializeField] Transform _respawnPoint;

        void Awake()
        {
            _penaltyArea.OnTriggerEnterAsObservable().Subscribe(c => Respawn(c));
        }

        void Respawn(Collider other)
        {
            if (other.TryGetComponent(out Player p))
            {
                p.Teleport(_respawnPoint.position);
            }
        }
    }
}
