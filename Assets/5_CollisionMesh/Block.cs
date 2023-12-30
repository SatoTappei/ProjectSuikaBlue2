using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.CollisionMesh
{
    public class Block : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }
    }
}
