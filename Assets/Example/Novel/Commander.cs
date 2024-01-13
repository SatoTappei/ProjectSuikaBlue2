using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Novel
{
    public class Commander : MonoBehaviour
    {
        [SerializeField] Character _centerCharacter;
        [SerializeField] Character _leftCharacter;
        [SerializeField] Character _rightCharacter;

        /// <summary>
        /// シーン開始前もしくは終了時にコマンドでイベントする各UIを初期化
        /// </summary>
        public void ResetAll()
        {
            _centerCharacter.Init();
            _leftCharacter.Init();
            _rightCharacter.Init();
        }

        /// <summary>
        /// コマンドを実行するイベントに変換
        /// </summary>
        public async UniTask RunAsync(string[] commands, CancellationToken skipToken)
        {
            List<Queue<UniTask>> tasks = new();
            for (int i = 0; i < commands.Length; i++)
            {
                string[] split = commands[i].Split();

                // 前のイベントに続けて実行されるかチェック
                bool isAppend = split[0][0] == '+';
                if (isAppend) split[0] = split[0].Substring(1);

                // 先頭のコマンドもしくは並列で実行されるイベントの場合は新たにキューを追加
                if (i == 0 || !isAppend) tasks.Add(new());

                // 続けて実行される場合は前のイベントのキューに、
                // 並列で実行する場合は新たに追加されたキューにイベントを追加
                AddEvent(tasks[^1], split, skipToken);
            }

            // 並列で実行されるキューから1つずつ取り出して実行を繰り返す
            while (true)
            {
                // 全てのキューが空かをチェック
                bool isEmpty = true;
                List<UniTask> temp = new();
                foreach (Queue<UniTask> q in tasks)
                {
                    if (q.Count == 0) continue;
                    
                    temp.Add(q.Dequeue());
                    isEmpty = false;
                }

                if (isEmpty) break;

                // 並列で実行されるイベントが全て終わるまで待つ
                await temp;
            }
        }

        // コマンドに対応したイベントをキューに追加
        void AddEvent(Queue<UniTask> q, string[] command, CancellationToken skipToken)
        {
            if (command[0] == "キャラ表示") q.Enqueue(ShowCharacterAsync(command, skipToken));
            else if (command[0] == "キャラ消去") q.Enqueue(HideCharacterAsync(command, skipToken));
            else if (command[0] == "キャラ上げ") q.Enqueue(CharacterMoveToFrontAsync(command, skipToken));
            else if (command[0] == "キャラ下げ") q.Enqueue(CharacterMoveToBackAsync(command, skipToken));
            else if (command[0] == "キャラジャンプ") q.Enqueue(CharacterJumpAsync(command, skipToken));
            else Debug.LogWarning("対応するイベントが無い: " + command[0]);
        }

        // キャラ表示
        UniTask ShowCharacterAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            string character = split[2];
            float duration = float.Parse(split[3]);
            return UniTask.Defer(
                () => CharacterFrame(position).FadeInAsync(SelectCharacter(character), duration, skipToken));
        }

        // キャラ消去
        UniTask HideCharacterAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            float duration = float.Parse(split[2]);
            return UniTask.Defer(
                () => CharacterFrame(position).FadeOutAsync(duration, skipToken));
        }

        // キャラ上げ
        UniTask CharacterMoveToFrontAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            // 必要に応じて時間に対応させる
            return UniTask.Defer(() 
                => CharacterFrame(position).SetOnFrontColorAsync(skipToken));
        }

        // キャラ下げ
        UniTask CharacterMoveToBackAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            // 必要に応じて時間に対応させる
            return UniTask.Defer(
                () => CharacterFrame(position).SetOnBackColorAsync(skipToken));
        }

        // キャラジャンプ
        UniTask CharacterJumpAsync(string[] split, CancellationToken skipToken)
        {
            string position = split[1];
            float duration = float.Parse(split[2]);
            int count = int.Parse(split[3]);
            return UniTask.Defer(
                () => CharacterFrame(position).JumpAsync(duration, count, skipToken));
        }

        Character CharacterFrame(string key)
        {
            if (key == "左") return _leftCharacter;
            else if (key == "中央") return _centerCharacter;
            else if (key == "右") return _rightCharacter;
            else throw new System.ArgumentException("対応するフレームが無い: " + key);
        }

        CharacterType SelectCharacter(string key)
        {
            if (key == "たかし") return CharacterType.Boy;
            else if (key == "たかしこ") return CharacterType.Girl;
            else if (key == "らん豚") return CharacterType.Oink;
            else throw new System.ArgumentException("対応するキャラクターが無い: " + key);
        }
    }
}
