using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Novel
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] MessagePrinter _printer;
        [SerializeField] Commander _commander;

        void Start()
        {
            SceneFlow sceneFlow = YamlReader.Deserialize();
            RunAsync(sceneFlow, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid RunAsync(SceneFlow flow, CancellationToken token)
        {
            IReadOnlyList<string[]> commands = flow.ToCommands();
            LineContent[] lines = flow.ToLineContents();

            int index = 0;
            while (index < flow.Sequence.Length)
            {
                CancellationTokenSource cts = new();

                _printer?.ShowMessage(lines[index].Line, lines[index].Name);
                _commander?.RunAsync(commands[index], cts.Token).Forget();

                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
                if (_printer.IsPrinting)
                {
                    _printer.Skip();
                    await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
                }

                index++;
                cts.Cancel();

                // キャンセルされた際のイベントの反映を行うので1フレーム待たないといけない
                await UniTask.Yield(token);
            }
        }
    }
}