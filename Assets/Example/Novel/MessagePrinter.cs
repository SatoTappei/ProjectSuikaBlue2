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

        bool _isSkipRequested; // スキップが要求されたかどうか

        /// <summary>
        /// 文字表示アニメーション中かどうか。
        /// </summary>
        public bool IsPrinting { get; private set; }

        /// <summary>
        /// 指定のメッセージを表示する。
        /// </summary>
        public void ShowMessage(string message, string name)
        {
            // メッセージが null なら例外出す。
            if (message is null) throw new ArgumentNullException(nameof(message));

            _nameUi.text = name;
            StartCoroutine(ShowMessageCoroutine(message));
        }

        // メッセージ再生
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
        /// 現在再生中の文字出力を省略する。
        /// </summary>
        public void Skip()
        {
            _isSkipRequested = true;
        }
    }
}