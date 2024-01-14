using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using VContainer;
using VContainer.Unity;

namespace PSB.Game
{
    public class Player : MonoBehaviour
    {
        public enum Forward { East, West, South, North, }

        [SerializeField] Rigidbody _rigidbody;
        [Header("���E�̃��C�L���X�g�̎n�_")]
        [SerializeField] Transform _eye;
        [Header("�����Ă�������̊")]
        [SerializeField] Transform _body;

        GameState _gameState;

        [Inject]
        void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

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

                Debug.Log("�����[");

                // ����
                Vector2Int input = default;
                if (msg.InputKeys.Contains(KeyCode.A)) { input.x--; }
                if (msg.InputKeys.Contains(KeyCode.S)) { input.y--; }
                if (msg.InputKeys.Contains(KeyCode.D)) { input.x++; }
                if (msg.InputKeys.Contains(KeyCode.W)) { input.y++; }
                // ���͂���܂ł���Ă�
                if (input == default) { await UniTask.Yield(token); continue; }

                // �W�����v�L�[�����͂���Ă����ꍇ�͂��̕����ɃW�����v
                // ���͂���Ă��Ȃ��ꍇ�͂��̕����Ɉړ�
                if (msg.InputKeys.Contains(KeyCode.Space)) { await JumpAsync(input); }
                else { await MoveAsync(input, token); }



                // ���C�L���X�g�ŃX�e�[�W�̉��ɗ����Ă��邩����
                //OnFloorBorder = Physics.Raycast(_eye.transform.position, _eye.forward + Vector3.down, 1);

                await UniTask.Yield(token);
            }
        }

        // �����ɔ��
        async UniTask JumpAsync(Vector2Int input)
        {
            _rigidbody.AddForce(Vector3.up + new Vector3(input.x, 0, 0), ForceMode.Impulse);
        }

        // 1�b�Ԉړ�
        async UniTask MoveAsync(Vector2Int input, CancellationToken token)
        {
            transform.forward = new Vector3(input.x, 0, input.y);
            for (int i = 0; i <= 60; i++)
            {
                _rigidbody.velocity = new Vector3(input.x, 0, input.y);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }
            _rigidbody.velocity = Vector3.zero;
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
