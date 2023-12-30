using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] Button _pauseButton;
        [SerializeField] Button _resultButton;

        GameContext _context;

        [Inject]
        void Construct(GameContext context)
        {
            _context = context;
        }

        void Awake()
        {
            if (_pauseButton != null) _pauseButton.onClick.AddListener(Pause);
            if (_resultButton != null) _resultButton.onClick.AddListener(ToResultScene);
        }

        void Pause()
        {
            if (_context == null) return;
            _context.IsPause = !_context.IsPause;
        }

        void ToResultScene()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.LoadAdditiveResultSceneAsync().Forget();
        }
    }
}
