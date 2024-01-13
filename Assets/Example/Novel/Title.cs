using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Novel
{
    public class Title : MonoBehaviour
    {
        [System.Serializable]
        class Episode
        {
            [SerializeField] string _label;
            [SerializeField] int _number;
            [SerializeField] Button _button;

            public string Label => _label;
            public int Number => _number;
            public Button Button => _button;
        }

        [SerializeField] Button _continue;
        [SerializeField] Episode[] _episodes;
        [SerializeField] CanvasGroup _root;

        /// <summary>
        /// 決定したボタンに応じたセーブデータを返す
        /// </summary>
        public async UniTask<SaveData> SubmitEpisodeAsync(CancellationToken token)
        {
            List<AsyncUnityEventHandler> handles = new();
            // 各話を最初から読むボタン
            for (int i = 0; i < _episodes.Length; i++)
            {
                handles.Add(_episodes[i].Button.onClick.GetAsyncEventHandler(token));
            }
            // 続きからボタン
            handles.Add(_continue.onClick.GetAsyncEventHandler(token));

            int submit = await UniTask.WhenAny(handles.Select(h => h.OnInvokeAsync()));
            if (0 <= submit && submit < _episodes.Length)
            {
                // 各話の最初からのセーブデータを作って返す
                return new() { Episode = _episodes[submit].Number, Index = 0 };
            }
            else
            {
                // セーブデータをロードして返す
                return SaveManager.Load();
            }
        }

        /// <summary>
        /// UIを全部表示してタイトル画面の状態をリセットする
        /// </summary>
        public async UniTask ShowUiAsync(CancellationToken token)
        {
            _root.alpha = 1;
            _root.interactable = true;
            _root.blocksRaycasts = true;

            await UniTask.Yield(token);
        }

        /// <summary>
        /// UIを全部隠して次のシーンに遷移可能な状態にする
        /// </summary>
        public async UniTask HideUiAsync(CancellationToken token)
        {
            _root.alpha = 0;
            _root.interactable = false;
            _root.blocksRaycasts = false;

            await UniTask.Yield(token);
        }
    }
}
