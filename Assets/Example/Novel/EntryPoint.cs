using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

namespace PSB.Novel
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] Title _title;
        [SerializeField] MessagePrinter _printer;
        [SerializeField] Commander _commander;
        [SerializeField] Button _clickInput;
        [SerializeField] BackLog _backLog;

        public int Index { get; private set; }
        public int Episode { get; private set; }

        void Start()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            LoopAsync(token).Forget();
        }

        async UniTask LoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SaveData save = await _title.SubmitEpisodeAsync(token);

                Index = save.Index;
                Episode = save.Episode;
                Debug.Log(Index + " " + Episode);

                await _title.HideUiAsync(token);

                string yaml = YamlAddress.Get(save.Episode);
                SceneFlow sceneFlow = YamlReader.Deserialize(yaml);
                await PlaySceneAsync(sceneFlow, save.Index, token);

                await _title.ShowUiAsync(token);
            }
        }

        async UniTask PlaySceneAsync(SceneFlow flow, int startIndex, CancellationToken token)
        {
            IReadOnlyList<string[]> commands = flow.ToCommands();
            LineContent[] lines = flow.ToLineContents();

            Index = startIndex;
            CancellationTokenSource cts = new();
            while (Index < flow.Sequence.Length)
            {
                cts = new();

                _printer?.ShowMessage(lines[Index].Line, lines[Index].Name);
                _backLog?.Add(lines[Index].Line, lines[Index].Name);
                _commander?.RunAsync(commands[Index], cts.Token).Forget();

                await OnClickAsync(token);
                if (_printer.IsPrinting)
                {
                    _printer.Skip();
                    await OnClickAsync(token);
                }

                Index++;
                cts.Cancel();

                // キャンセルされた際のイベントの反映を行うので1フレーム待たないといけない
                await UniTask.Yield(token);
            }

            if (!cts.IsCancellationRequested)
            {
                Debug.LogWarning("シーン終了時にコマンドのスキップ用トークンがキャンセルされていない");
            }

            _commander.ResetAll();
            _backLog.Release();
        }

        // クリックまで待つ
        async UniTask<Unit> OnClickAsync(CancellationToken token)
        {
            return await _clickInput.OnClickAsObservable().ToUniTask(useFirstValue: true, token);
        }
    }
}