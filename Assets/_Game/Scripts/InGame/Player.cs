using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    // このスクリプトがアタッチされたオブジェクトが移動と回転どちらも行う
    public class Player : MonoBehaviour
    {
        [Header("地面からの高さ")]
        [SerializeField] float _groundOffset;

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // 地面に立たせる
            transform.Translate(Vector3.up * _groundOffset);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            Direction forward = Direction.North;
            Transform transform = this.transform;
            while (!token.IsCancellationRequested)
            {
                using (CancellationTokenSource skipTokenSource = new())
                {
                    // 入力のメッセージが飛んでくるまで待機
                    KeyInputMessage msg = await MessageBroker.Default
                        .Receive<KeyInputMessage>().ToUniTask(useFirstValue: true, token);

                    // 移動もしくは回転
                    if (msg.IsMoveKey(out KeyCode moveKey))
                    {
                        Vector3 move = moveKey.ToNormalizedDirectionVector(forward);
                        await transform.MoveAsync(move, 0.5f, skipTokenSource.Token);
                    }
                    else if (msg.IsRotateKey(out KeyCode rotKey))
                    {
                        float rot = rotKey.To90DegreeRotateAngle();
                        await transform.RotateAsync(rot, 0.5f, skipTokenSource.Token);
                        forward = rotKey.ToTurnedDirection(forward);
                    }
                }
            }
        }
    }
}
