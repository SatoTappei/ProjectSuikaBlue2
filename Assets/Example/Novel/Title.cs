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
        /// ���肵���{�^���ɉ������Z�[�u�f�[�^��Ԃ�
        /// </summary>
        public async UniTask<SaveData> SubmitEpisodeAsync(CancellationToken token)
        {
            List<AsyncUnityEventHandler> handles = new();
            // �e�b���ŏ�����ǂރ{�^��
            for (int i = 0; i < _episodes.Length; i++)
            {
                handles.Add(_episodes[i].Button.onClick.GetAsyncEventHandler(token));
            }
            // ��������{�^��
            handles.Add(_continue.onClick.GetAsyncEventHandler(token));

            int submit = await UniTask.WhenAny(handles.Select(h => h.OnInvokeAsync()));
            if (0 <= submit && submit < _episodes.Length)
            {
                // �e�b�̍ŏ�����̃Z�[�u�f�[�^������ĕԂ�
                return new() { Episode = _episodes[submit].Number, Index = 0 };
            }
            else
            {
                // �Z�[�u�f�[�^�����[�h���ĕԂ�
                return SaveManager.Load();
            }
        }

        /// <summary>
        /// UI��S���\�����ă^�C�g����ʂ̏�Ԃ����Z�b�g����
        /// </summary>
        public async UniTask ShowUiAsync(CancellationToken token)
        {
            _root.alpha = 1;
            _root.interactable = true;
            _root.blocksRaycasts = true;

            await UniTask.Yield(token);
        }

        /// <summary>
        /// UI��S���B���Ď��̃V�[���ɑJ�ډ\�ȏ�Ԃɂ���
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
