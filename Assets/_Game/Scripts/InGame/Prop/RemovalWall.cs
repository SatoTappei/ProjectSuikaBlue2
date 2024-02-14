using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public class RemovalWall : MonoBehaviour, IDungeonParts
    {
        [Header("íœ‚ÉŒ©‚½–Ú‚ğ®‚¦‚é’Œ")]
        [SerializeField] GameObject _pillar;

        void IDungeonParts.Remove()
        {
            gameObject.SetActive(false);
            if (_pillar != null) _pillar.SetActive(true);
        }
    }
}
