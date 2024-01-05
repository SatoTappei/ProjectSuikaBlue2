using System.Collections;
using UnityEngine;

namespace PSB.Novel
{
    public class MessageSequencer : MonoBehaviour
    {
        [SerializeField] MessagePrinter _printer = default;

        /// <summary>
        /// このシーンで再生する会話を渡して開始
        /// </summary>
        public void Run(LineContent[] lines)
        {
            StartCoroutine(RunCoroutine(lines));
        }

        IEnumerator RunCoroutine(LineContent[] messages)
        {
            if (messages is null or { Length: 0 }) { yield break; }

            var index = 0;
            while (index < messages.Length)
            {
                if (_printer.IsPrinting) { _printer.Skip(); }
                else { _printer?.ShowMessage(messages[index].Line, messages[index++].Name); }

                while (!Input.GetMouseButtonDown(0)) { yield return null; }
                yield return null;
            }
        }
    }
}