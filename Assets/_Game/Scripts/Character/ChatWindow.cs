using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class ChatWindow : MonoBehaviour
    {
        [Header("�`���b�g��ʂ̊J��")]
        [SerializeField] Button _switchButton;
        [SerializeField] Transform _window;
        [SerializeField] float _closedWidth = -429.0f;
        [SerializeField] float _OpenedWidth = 19.0f;
        [SerializeField] float _animationSpeed = 1.0f;
        [SerializeField] string _closedLetter = ">";
        [SerializeField] string _opendLetter = "<";

        CancellationToken _token;
        bool _isOpened;

        void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        void Start()
        {
            StateChange(_closedWidth, 0, _closedLetter);
            _switchButton.onClick.AddListener(Switch);
        }

        // �`���b�g��ʂ̊J��
        void Switch()
        {
            if (_isOpened) StateChange(_closedWidth, _animationSpeed, _closedLetter);
            else StateChange(_OpenedWidth, _animationSpeed, _opendLetter);

            _isOpened = !_isOpened;
        }

        // �`���b�g��ʂ̏�Ԃ�؂�ւ���
        void StateChange(float to, float duration, string letter)
        {
            WindowAnimationAsync(to, duration, _token).Forget();
            _switchButton.GetComponentInChildren<Text>().text = letter;
        }

        // �`���b�g��ʊJ�̃A�j���[�V����
        async UniTaskVoid WindowAnimationAsync(float to, float duration, CancellationToken token)
        {
            Vector3 a = _window.localPosition;
            Vector3 b = new Vector3(to, a.y, a.z);

            // �A�j���[�V�������̓{�^���������Ȃ�
            _switchButton.interactable = false;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _window.localPosition = Vector3.Lerp(a, b, t / duration);
                await UniTask.Yield();
            }
            _window.localPosition = b;
            _switchButton.interactable = true;
        }
    }
}
