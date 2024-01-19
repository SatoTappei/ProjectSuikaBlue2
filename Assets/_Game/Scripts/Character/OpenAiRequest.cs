using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PSB.Game
{
    #region API�Ƃ̂������i�[����N���X
    [System.Serializable]
    public class ApiRequestMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ApiResponseMessage
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
            public ApiRequestMessage message;
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

    [System.Serializable]
    public class ApiConfig
    {
        public string model;
        public List<ApiRequestMessage> messages;
    }
    #endregion

    public class OpenAiRequest
    {
        const string ApiKey = "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";
        const string Version = "gpt-4-1106-preview";
        const string OldVersion = "gpt-3.5-turbo";

        List<ApiRequestMessage> _messages = new();

        public OpenAiRequest(string content)
        {
            _messages.Add(new ApiRequestMessage()
            {
                role = "system",
                content = content
            });
        }

        public async UniTask<string> RequestAsync(string message)
        {
            // ���N�G�X�g��JSO�`���̕�����ɕϊ�
            _messages.Add(new ApiRequestMessage { role = "user", content = message });
            ApiConfig config = new ApiConfig()
            {
                model = Version,
                messages = _messages,
            };
            string json = JsonUtility.ToJson(config);

            // API���N�G�X�g�̌��ʂ��i�[
            using UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            // API���N�G�X�g�̃w�b�_�[��ݒ�
            Dictionary<string, string> header = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + ApiKey}, // API�L�[�̔F��
                {"Content-type", "application/json"},  // Json�`���Ń��N�G�X�g
            };
            foreach (KeyValuePair<string, string> h in header)
            {
                request.SetRequestHeader(h.Key, h.Value);
            }

            // ���N�G�X�g���M
            await request.SendWebRequest();

            // ����
            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new UnityWebRequestException(request);
            }
            else
            {
                ApiResponseMessage response = JsonUtility.FromJson<ApiResponseMessage>(request.downloadHandler.text);
                _messages.Add(response.choices[0].message);
                return response.choices[0].message.content;
            }
        }
    }
}
