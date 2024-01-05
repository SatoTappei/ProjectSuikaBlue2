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
        /// 現在、文字表示アニメーション中かどうか。
        /// </summary>
        public bool IsPrinting { get; private set; }

        /// <summary>
        /// 指定のメッセージを表示する。
        /// </summary>
        /// <param name="message">テキストとして表示するメッセージ。</param>
        public void ShowMessage(string message, string name)
        {
            // メッセージが null なら例外出す。
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
                yield return null; // 1フレーム分の処理
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
        private bool _isSkipRequested; // スキップが要求されたかどうか
    }
}