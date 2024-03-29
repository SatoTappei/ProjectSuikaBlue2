using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public class RemovalWall : MonoBehaviour, IDungeonParts
    {
        [Header("削除時に見た目を整える柱")]
        [SerializeField] GameObject _pillar;

        void IDungeonParts.Remove()
        {
            gameObject.SetActive(false);
            if (_pillar != null) _pillar.SetActive(true);
        }
    }
}
