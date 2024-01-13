using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;

public struct PlayerControlMessage
{
    public KeyCode Key;
}

/// <summary>
/// Rotation�̒l��(0,0,0)�̎���Forward��k�Ƃ���������k
/// </summary>
public enum PlayerForward
{
    East,
    West,
    South,
    North,
}

namespace PSB.Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] Transform _eye;

        public bool OnFloorBorder { get; private set; }
        public PlayerForward Forward { get; private set; }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // ���b�Z�[�W�����ł���܂őҋ@
                PlayerControlMessage msg = await MessageBroker.Default
                    .Receive<PlayerControlMessage>().ToUniTask(useFirstValue: true, token);

                // ����
                Vector2Int input = default;
                if (msg.Key == KeyCode.A) { input.x--; Forward = PlayerForward.West; }
                if (msg.Key == KeyCode.S) { input.y--; Forward = PlayerForward.South; }
                if (msg.Key == KeyCode.D) { input.x++; Forward = PlayerForward.East; }
                if (msg.Key == KeyCode.W) { input.y++; Forward = PlayerForward.North; }
                // ���͂���܂ł���Ă�
                if (input == Vector2Int.zero) { await UniTask.Yield(token); continue; }

                // ���͂ɉ�����1��ړ�
                transform.forward = new Vector3(input.x, 0, input.y);
                for (int i = 0; i <= 60; i++)
                {
                    _rigidbody.velocity = new Vector3(input.x, 0, input.y);
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                }
                _rigidbody.velocity = Vector3.zero;

                // ���C�L���X�g�ŃX�e�[�W�̉��ɗ����Ă��邩����
                OnFloorBorder = Physics.Raycast(_eye.transform.position, _eye.forward + Vector3.down, 1);

                await UniTask.Yield(token);
            }
        }

        void OnDrawGizmos()
        {
            if (_eye != null)
            {
                Vector3 ray = _eye.forward + Vector3.down;
                Gizmos.DrawRay(_eye.transform.position, ray);
            }
        }
    }
}
