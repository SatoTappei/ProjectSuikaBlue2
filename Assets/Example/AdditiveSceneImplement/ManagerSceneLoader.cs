using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSB.Architect
{
    /// <summary>
    /// このスクリプトをアタッチしたGameObjectを各シーンに配置する。
    /// どのシーンから開始してもManagerシーンが存在した状態になる。
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