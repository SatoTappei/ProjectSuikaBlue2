using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            LoadDefaultSceneAsync().Forget();
        }

        async UniTask LoadDefaultSceneAsync()
        {
            AsyncOperation inGame = SceneManager.LoadSceneAsync(Const.InGameSceneName, LoadSceneMode.Additive);
            AsyncOperation character = SceneManager.LoadSceneAsync(Const.CharacterSceneName, LoadSceneMode.Additive);
            await UniTask.WhenAll(inGame.ToUniTask(), character.ToUniTask());
        }
    }
}
