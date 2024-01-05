using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

namespace PSB.Architect
{
    /// <summary>
    /// シングルトンなのでどのシーンからでも呼び出すことが出来る
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // 重複して読み込まないように既にシーンを読み込んでいるかを保持
        // Manager:0 Architect:1 Graphics:2 Result:3
        bool[] _isLoaded = new bool[4];

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        async void Start()
        {
            await LoadDefaultSceneAsync();
        }

        async UniTask LoadDefaultSceneAsync()
        {
            AsyncOperation arc = SceneManager.LoadSceneAsync(Const.ArchitectSceneName, LoadSceneMode.Additive);
            AsyncOperation gra = SceneManager.LoadSceneAsync(Const.GraphicsSceneName, LoadSceneMode.Additive);
            AsyncOperation rsl = SceneManager.LoadSceneAsync(Const.ResultSceneName, LoadSceneMode.Additive);
            await UniTask.WhenAll(arc.ToUniTask(), gra.ToUniTask(), rsl.ToUniTask());
            _isLoaded[1] = true;
            _isLoaded[2] = true;
            _isLoaded[3] = true;

            AsyncOperation rslUn = SceneManager.UnloadSceneAsync(Const.ResultSceneName);
            await rslUn;
            _isLoaded[3] = false;
        }

        /// <summary>
        /// リザルトシーンを追加で読み込む
        /// </summary>
        public async UniTask LoadAdditiveResultSceneAsync()
        {
            if (_isLoaded[3]) return;
            await SceneManager.LoadSceneAsync(Const.ResultSceneName, LoadSceneMode.Additive);
            _isLoaded[3] = true;
        }

        /// <summary>
        /// 他のシーンからインスタンスが存在するかチェック
        /// </summary>
        public void Check()
        {
            Debug.Log("存在確認");
        }
    }
}
