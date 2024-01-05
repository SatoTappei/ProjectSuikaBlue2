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
        /// コマンドを実行するイベントに変換
        /// </summary>
        public async UniTask RunAsync(string[] commands, CancellationToken skipToken)
        {
            List<List<UniTask>> tasks = new List<List<UniTask>>();
            tasks.Add(new List<UniTask>()); // 現状、並列でのイベント実行しか対応していない

            foreach (string c in commands)
            {
                string[] split = c.Split();

                // 先頭に+が付いていた場合はシーケンシャルに実行される
                if (split[0][0] == '+')
                {
                    split[0] = split[0].Substring(1);
                    // シーケンシャルにする魔法
                }

                if (split[0] == "キャラ表示") tasks[0].Add(ShowCharacterAsync(split, skipToken));
                else if (split[0] == "キャラ消去") tasks[0].Add(HideCharacterAsync(split, skipToken));
                else if (split[0] == "キャラ上げ") tasks[0].Add(CharacterMoveToFrontAsync(split, skipToken));
                else if (split[0] == "キャラ下げ") tasks[0].Add(CharacterMoveToBackAsync(split, skipToken));
                else if (split[0] == "キャラジャンプ") tasks[0].Add(CharacterJumpAsync(split, skipToken));
                else Debug.LogWarning("対応するイベントが無い: " + split[0]);
            }

            foreach (List<UniTask> parallel in tasks)
            {
                await parallel;
            }
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
