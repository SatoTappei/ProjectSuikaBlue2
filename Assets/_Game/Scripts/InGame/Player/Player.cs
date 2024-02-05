using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;

namespace PSB.Game
{
    // このスクリプトがアタッチされたオブジェクトが移動と回転どちらも行う
    public class Player : MonoBehaviour
    {
        GameState _gameState;
        PlayerParameterSettings _settings;

        [Inject]
        void Construct(GameState gameState, PlayerParameterSettings settings)
        {
            _gameState = gameState;
            _settings = settings;
        }

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // 地面に立たせる
            transform.Translate(Vector3.up * _settings.GroundOffset);
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
                        float speed = _settings.MoveSpeed;
                        await transform.MoveAsync(move, speed, skipTokenSource.Token);
                    }
                    else if (msg.IsRotateKey(out KeyCode rotKey))
                    {
                        float rot = rotKey.To90DegreeRotateAngle();
                        float speed = _settings.RotateSpeed;
                        await transform.RotateAsync(rot, speed, skipTokenSource.Token);
                        forward = rotKey.ToTurnedDirection(forward);
                    }
                }
            }
        }
    }
}

// TODO:プレイヤーにダンジョンのSOが渡されているので、セルの大きさに合わせて移動するように変更する