using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerDirectionalLight : MonoBehaviour
{
    [Header("Start���ɃI�t�ɂ���")]
    [SerializeField] GameObject _light;

    void Start()
    {
        // �Q�[���J�n���Ƀ��C�g���I�t�ɂ����ꍇ��
        // �q�G�����L�[�Ń��C�g���I�t�ɂ����ꍇ�Ń��C�e�B���O���ς���Ă��܂��B
        _light.SetActive(false);
    }
}
