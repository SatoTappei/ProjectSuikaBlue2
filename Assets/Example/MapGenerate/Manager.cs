using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
//using PSB.Game; // OpenAIへのリクエスト用

namespace PSB.MapGenerate
{
    public class Manager : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
            public string Letter;
            public GameObject Prefab;
        }

        [SerializeField] TextAsset _rule;
        [Header("デバッグ用: テキストからマップを読み込む")]
        [SerializeField] TextAsset _debugMap;
        [SerializeField] bool _useDebugMap;
        [Header("文字に対応したオブジェクト")]
        [SerializeField] Data[] _data;

        Dictionary<string, GameObject> _dict;
        List<GameObject> _list;

        void Awake()
        {
            _dict = _data.ToDictionary(d => d.Letter, d => d.Prefab);
            _list = new();
        }

        async void Start()
        {
            if (_useDebugMap) Generate(_debugMap.ToString().Split("\n"));
            else
            {
                using (CancellationTokenSource cts = new())
                {
                    //OpenAiRequest ai = new(_rule.ToString());
                    //string response = await ai.RequestAsync("生成してください");

                    //if (cts.IsCancellationRequested) return;
                    //Debug.Log(response);
                    //Generate(Split(response));
                }
            }
        }
        
        void OnDestroy()
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                Destroy(_list[i]);
            }

            _list.Clear();
        }

        string[] Split(string response)
        {
            return response.Split("\n");
        }

        void Generate(string[] map)
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int k = 0; k < map[i].Length; k++)
                {
                    string key = map[i][k].ToString();
                    if (!_dict.ContainsKey(key)) continue;
                    Quaternion rot = Quaternion.Euler(0, Random.Range(0, 5) * 90, 0);
                    GameObject g = Instantiate(_dict[key], new Vector3(k, 0, i), rot);
                    _list.Add(g);
                }
            }
        }
    }
}
