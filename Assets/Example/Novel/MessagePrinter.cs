using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PSB.Novel
{
    public class MessagePrinter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _nameUi = default;

        [SerializeField]
        private TMP_Text _textUi = default;

        [SerializeField]
        private float _speed = 1.0F;

        /// <summary>
        /// ���݁A�����\���A�j���[�V���������ǂ����B
        /// </summary>
        public bool IsPrinting { get; private set; }

        /// <summary>
        /// �w��̃��b�Z�[�W��\������B
        /// </summary>
        /// <param name="message">�e�L�X�g�Ƃ��ĕ\�����郁�b�Z�[�W�B</param>
        public void ShowMessage(string message, string name)
        {
            // ���b�Z�[�W�� null �Ȃ��O�o���B
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _nameUi.text = name;
            StartCoroutine(ShowMessageCoroutine(message));
        }

        private IEnumerator ShowMessageCoroutine(string message)
        {
            if (_textUi is null || message is null) { yield break; }

            _isSkipRequested = false;
            _textUi.text = "";
            IsPrinting = true;

            var index = 0;
            var elapsed = 0F;
            var interval = _speed / message.Length;
            while (index < message.Length && !_isSkipRequested)
            {
                elapsed += Time.deltaTime;
                if (elapsed > interval)
                {
                    elapsed = 0;
                    _textUi.text += message[index++];
                }
                yield return null; // 1�t���[�����̏���
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
        private bool _isSkipRequested; // �X�L�b�v���v�����ꂽ���ǂ���
    }
}