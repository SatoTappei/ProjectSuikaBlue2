using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// �ȉ��̃T�C�g����R�s�y
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

    //ChatGPT API��Request�𑗂邽�߂�JSON�p�N���X
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
                content = "���C�ȏ��̎q�ɂȂ肫���Ď��Ɖ�b���Ă��������B" +
                "�E�Ƃ̓��C�h�A�N���13�΂ł��B���O�͖����̂ŕ����ꂽ��u�閧�ł���v�Ɠ����Ă��������B" +
                "���X�|���X�̓����t���[�Y�A�Œ��ł�10�������x�ł��肢���܂��B�B�ȉ��͉�b�̃T���v���ł�" +
                "���u����ɂ��́v���Ȃ��u����ɂ��킟�`�v" +
                "���u�������H�v���Ȃ��u15���ł��`�v" +
                "���u�΂��΂��v���Ȃ��u����ꂳ�܂ł����v"
            });
        }

        public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage)
        {
            //���͐���AI��API�̃G���h�|�C���g��ݒ�
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            _messages.Add(new ChatGPTMessageModel { role = "user", content = userMessage });

            //OpenAI��API���N�G�X�g�ɕK�v�ȃw�b�_�[����ݒ�
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + ApiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };

            //���͐����ŗ��p���郂�f����g�[�N������A�v�����v�g���I�v�V�����ɐݒ�
            var options = new ChatGPTCompletionRequestModel()
            {
                //model = "gpt-3.5-turbo",
                model = "gpt-4-1106-preview",
                messages = _messages
            };
            var jsonOptions = JsonUtility.ToJson(options);

            Debug.Log("����:" + userMessage);

            //OpenAI�̕��͐���(Completion)��API���N�G�X�g�𑗂�A���ʂ�ϐ��Ɋi�[
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