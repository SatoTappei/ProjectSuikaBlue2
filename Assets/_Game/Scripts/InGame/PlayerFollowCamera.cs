using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    [SerializeField] Transform _player;

    Transform _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main.transform;
    }

    void FixedUpdate()
    {
        Vector3 to = _player.position;
        to.z = _mainCamera.position.z;
        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.position, to, 0.5f);
    }
}
