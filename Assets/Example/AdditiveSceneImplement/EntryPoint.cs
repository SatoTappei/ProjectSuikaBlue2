using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Architect
{
    public class EntryPoint : MonoBehaviour
    {
        void Start()
        {
            // Manager�V�[���ɂ���C���X�^���X���Q�Ƃ���
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Check();
            }
        }
    }
}