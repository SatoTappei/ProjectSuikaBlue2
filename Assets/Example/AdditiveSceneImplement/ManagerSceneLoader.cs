using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSB.Architect
{
    /// <summary>
    /// ���̃X�N���v�g���A�^�b�`����GameObject���e�V�[���ɔz�u����B
    /// �ǂ̃V�[������J�n���Ă�Manager�V�[�������݂�����ԂɂȂ�B
    /// </summary>
    public class ManagerSceneLoader : MonoBehaviour
    {
        public static bool Loaded { get; private set; }

        void Awake()
        {
            if (Loaded) return;
            Loaded = true;

            SceneManager.LoadScene(Const.ManagerSceneName, LoadSceneMode.Additive);
        }
    }
}