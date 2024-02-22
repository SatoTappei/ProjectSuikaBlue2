using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PSB.Novel
{
    public class MessagePrinter : MonoBehaviour
    {
        [SerializeField] TMP_Text _nameUi;
        [SerializeField] TMP_Text _textUi;
        [SerializeField] float _speed = 1.0F;

        bool _isSkipRequested; // �X�L�b�v���v�����ꂽ���ǂ���

        /// <summary>
        /// �����\���A�j���[�V���������ǂ����B
        /// </summary>
        public bool IsPrinting { get; private set; }

        /// <summary>
        /// �w��̃��b�Z�[�W��\������B
        /// </summary>
        public void ShowMessage(string message, string name)
        {
            // ���b�Z�[�W�� null �Ȃ��O�o���B
            if (message is null) throw new ArgumentNullException(nameof(message));

            _nameUi.text = name;
            StartCoroutine(ShowMessageCoroutine(message));
        }

        // ���b�Z�[�W�Đ�
        IEnumerator ShowMessageCoroutine(string message)
        {
            if (_textUi is null || message is null) yield break;

            _isSkipRequested = false;
            _textUi.text = "";
            IsPrinting = true;

            int index = 0;
            float elapsed = 0;
            float interval = _speed / message.Length;
            while (index < message.Length && !_isSkipRequested)
            {
                elapsed += Time.deltaTime;
                if (elapsed > interval)
                {
                    elapsed = 0;
                    _textUi.text += message[index++];
                }
                yield return null;
            }

            _textUi.text = message;
            IsPrinting = false;
        }

        /// <summary>
        /// ���ݍĐ����̕����o�͂��ȗ�����B
        /// </summary>
        public void Skip()
        {
            _isSkipRequested = true;
        }
    }
}