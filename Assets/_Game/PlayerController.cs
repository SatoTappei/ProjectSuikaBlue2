using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;

    void Start()
    {
        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTaskVoid UpdateAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Vector2Int input = default;
            if (Input.GetKeyDown(KeyCode.A)) input.x--;
            if (Input.GetKeyDown(KeyCode.S)) input.y--;
            if (Input.GetKeyDown(KeyCode.D)) input.x++;
            if (Input.GetKeyDown(KeyCode.W)) input.y++;

            if (input == Vector2Int.zero) { await UniTask.Yield(token); continue; }

            for (int i = 0; i <= 60; i++)
            {
                _rigidbody.velocity = new Vector3(input.x, 0, input.y);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }
            _rigidbody.velocity = Vector3.zero;

            await UniTask.Yield(token);
        }
    }
}
