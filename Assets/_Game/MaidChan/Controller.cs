using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] TextAsset _textAsset;
        [SerializeField] InputField _inputField;
        [SerializeField] Text _lineText;
        [SerializeField] Text _logText;
        [SerializeField] GameObject _face1;
        [SerializeField] GameObject _face2;
        [SerializeField] string _defaultLine = "";
        [SerializeField] int _messageSpeed = 5;
        [SerializeField] int _logMax = 10;
        [SerializeField] int _logLineMax = 20;

        Queue<string> _log;

        void Awake()
        {
            _log = new();
            _lineText.text = _defaultLine;
            _logText.text = "";
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            OpenAiRequest api = new(_textAsset.ToString());
            while (!token.IsCancellationRequested)
            {
                // 入力待ち
                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return), cancellationToken: token);

                if (_inputField.text == "") continue;
                
                string input = _inputField.text;
                _inputField.text = "";

                AddLog(input, "あなた: ");

                // ChatGPTにリクエスト
                string response = await api.RequestAsync(input);
                AddLog(response, "めいど: ");

                // 表情変える
                bool isSmile = Random.value <= 0.5f;
                _face1.SetActive(!isSmile);
                _face2.SetActive(isSmile);

                // 台詞として表示
                await PrintAsync(response, token);
                await UniTask.Yield();
            }
        }

        async UniTask PrintAsync(string line, CancellationToken token)
        {
            StringBuilder builder = new();

            _lineText.text = "";
            for (int i = 0; i < line.Length; i++)
            {
                builder.Append(line[i]);
                _lineText.text = builder.ToString();
                await UniTask.WaitForSeconds(1 / _messageSpeed, cancellationToken: token);
            }
            _lineText.text = line;
        }

        void AddLog(string str, string label)
        {
            str = str.Trim();
            string log = label + str;
            if (log.Length > _logLineMax)
            {
                log = log.Substring(0, _logLineMax);
                log += "...";
            }

            _log.Enqueue(log);
            if (_log.Count > _logMax) _log.Dequeue();

            StringBuilder builder = new();
            foreach (string s in _log)
            {
                builder.AppendLine(s);
            }

            _logText.text = builder.ToString();
        }
    }
}
