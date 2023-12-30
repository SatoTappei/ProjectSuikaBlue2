using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

namespace PSB.Architect
{
    /// <summary>
    /// �V���O���g���Ȃ̂łǂ̃V�[������ł��Ăяo�����Ƃ��o����
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // �d�����ēǂݍ��܂Ȃ��悤�Ɋ��ɃV�[����ǂݍ���ł��邩��ێ�
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
        /// ���U���g�V�[����ǉ��œǂݍ���
        /// </summary>
        public async UniTask LoadAdditiveResultSceneAsync()
        {
            if (_isLoaded[3]) return;
            await SceneManager.LoadSceneAsync(Const.ResultSceneName, LoadSceneMode.Additive);
            _isLoaded[3] = true;
        }

        /// <summary>
        /// ���̃V�[������C���X�^���X�����݂��邩�`�F�b�N
        /// </summary>
        public void Check()
        {
            Debug.Log("���݊m�F");
        }
    }
}
