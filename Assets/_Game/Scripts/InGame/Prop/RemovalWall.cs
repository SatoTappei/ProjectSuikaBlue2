using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public class RemovalWall : MonoBehaviour, IDungeonParts
    {
        [Header("�폜���Ɍ����ڂ𐮂��钌")]
        [SerializeField] GameObject _pillar;

        void IDungeonParts.Remove()
        {
            gameObject.SetActive(false);
            if (_pillar != null) _pillar.SetActive(true);
        }
    }
}
