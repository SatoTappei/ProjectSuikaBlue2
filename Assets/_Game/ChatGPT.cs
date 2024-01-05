using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// 以下のサイトからコピペ
// https://note.com/negipoyoc/n/n88189e590ac3
namespace PSB.Game
{
    [System.Serializable]
    public class ChatGPTMessageModel
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatGPTResponseModel
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public ChatGPTMessageModel message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }

    //ChatGPT APIにRequestを送るためのJSON用クラス
    [Serializable]
    public class ChatGPTCompletionRequestModel
    {
        public string model;
        public List<ChatGPTMessageModel> messages;
    }

    public class ChatGPT
    {
        const string ApiKey = "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";

        List<ChatGPTMessageModel> _messages = new();

        public ChatGPT()
        {
            _messages.Add(new()
            {
                role = "system",
                content = "健気な女の子になりきって私と会話してください。" +
                "職業はメイド、年齢は13歳です。名前は無いので聞かれたら「秘密です♪」と答えてください。" +
                "レスポンスはワンフレーズ、最長でも10文字程度でお願いします。。以下は会話のサンプルです" +
                "私「こんにちは」あなた「こんにちわぁ～」" +
                "私「今何時？」あなた「15時です～」" +
                "私「ばいばい」あなた「お疲れさまですっ」"
            });
        }

        public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage)
        {
            //文章生成AIのAPIのエンドポイントを設定
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            _messages.Add(new ChatGPTMessageModel { role = "user", content = userMessage });

            //OpenAIのAPIリクエストに必要なヘッダー情報を設定
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + ApiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };

            //文章生成で利用するモデルやトークン上限、プロンプトをオプションに設定
            var options = new ChatGPTCompletionRequestModel()
            {
                //model = "gpt-3.5-turbo",
                model = "gpt-4-1106-preview",
                messages = _messages
            };
            var jsonOptions = JsonUtility.ToJson(options);

            Debug.Log("自分:" + userMessage);

            //OpenAIの文章生成(Completion)にAPIリクエストを送り、結果を変数に格納
            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
                Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);
                _messages.Add(responseObject.choices[0].message);
                return responseObject;
            }
        }
    }
}