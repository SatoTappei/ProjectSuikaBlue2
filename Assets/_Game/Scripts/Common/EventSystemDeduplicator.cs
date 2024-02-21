using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���Z�V�[���ōĐ�����ۂɏd������EventSystem�����m���폜����B
/// </summary>
public class EventSystemDeduplicator : MonoBehaviour
{
    [Header("�����V�[���ōĐ������ۂɖ�����")]
    [SerializeField] EventSystem _eventSystem;

    // �V���O���g���I�Ȏg�������邽�߂̃t���O
    static bool _first = true;

    void Awake()
    {
        // �V���O���g�����ȃ`�F�b�N
        if (_first) _first = false;
        else
        {
            // �x�����o��̂�Listener�����łɍ폜���Ă���
            if (_eventSystem != null) Destroy(_eventSystem.gameObject);
        }
    }

    void OnDestroy()
    {
        _first = true;
    }
}
