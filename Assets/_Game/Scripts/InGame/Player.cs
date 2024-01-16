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
        public enum Forward { Left, Right }

        [SerializeField] Rigidbody _rigidbody;
        [Header("���E�̃��C�L���X�g�̎n�_")]
        [SerializeField] Transform _eye;
        [Header("�i������̃��C�L���X�g�̎n�_")]
        [SerializeField] Transform _kness;
        [Header("�����Ă�������̊")]
        [SerializeField] Transform _body;
        [Header("�ڒn����p�̃��C�L���X�g�̐ݒ�")]
        [SerializeField] float _groundingRadius = 0.5f;
        [SerializeField] float _groundingHeight = 0.1f;
        [SerializeField] Vector3 _groundingOffset;
        [Header("�W�����v�̐ݒ�")]
        [SerializeField] float _jumpInterval = 0.5f;
        [SerializeField] float _verticalJumpPower = 5.0f;
        [SerializeField] float _horizontalJumpPowr = 3.0f;
        [Header("���͔���p�̃��C�L���X�g�̐ݒ�")]
        [SerializeField] float _stageBorderRaycastLength = 1.0f;
        [SerializeField] float _holeFrontRaycastLength = 1.0f;
        [SerializeField] float _stepFrontRaycastLength = 2.0f;

        GameState _gameState;
        Transform _transform;

        [Inject]
        void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        void Awake()
        {
            _transform = transform;
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

                // �ǂ̈ړ��L�[�����͂��ꂽ��
                Vector2Int input = default;
                if (msg.KeyDownA) { input.x--; }
                if (msg.KeyDownD) { input.x++; }

                Rotation(input);

                // �W�����v or �ړ�
                if (msg.KeyDownSpace) { await JumpAsync(input, token); }
                else { await MoveAsync(input, token); }

                Idle();
                CheckSurroundings();

                await UniTask.Yield(token);
            }
        }

        // �����Ɍ���
        void Rotation(Vector2Int input)
        {
            _body.forward = new Vector3(input.x, 0, input.y);

            if (input == Vector2Int.left) _gameState.Forward = Forward.Left;
            else if (input == Vector2Int.right) _gameState.Forward = Forward.Right;
        }

        // �����ɔ��
        async UniTask JumpAsync(Vector2Int input, CancellationToken token)
        {
            Vector3 force = Vector3.up * _verticalJumpPower + Vector3.right * input.x * _horizontalJumpPowr;
            _rigidbody.AddForce(force, ForceMode.Impulse);

            // �W�����v���ɉ������ɗ͂����������邱�ƂŁA�i���Ɉ����������Ă����z���邱�Ƃ��o����B
            for (float f = 0; f < _jumpInterval; f += Time.fixedDeltaTime)
            {
                Vector3 velo = _rigidbody.velocity;
                velo.x = input.x * _horizontalJumpPowr;
                _rigidbody.velocity = velo;
                // �W�����v�������̃t���[���ł̓��C�L���X�g���n�ʂ��痣��Ȃ��̂ŁA����܂ł̃N�[���^�C����݂���B
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: token);
            }
 
            await UniTask.WaitUntil(IsGrounding, cancellationToken: token);
        }

        // �ڒn����
        bool IsGrounding()
        {
            // Z�������ɂ͓����Ȃ��̂�2�ӏ����肷��Α��v
            return Linecast(Vector3.left * _groundingRadius) ||
                   Linecast(Vector3.right * _groundingRadius);

            // ���S�ʒu��������������炵���ʒu�ɏc�����̐��L���X�g
            bool Linecast(Vector3 side)
            {
                Vector3 center = _transform.position + side + _groundingOffset;
                Vector3 halfHeight = _transform.up * (_groundingHeight / 2);
                Physics.Linecast(center + halfHeight, center - halfHeight, out RaycastHit hit, Const.FootingLayer);
                return hit.collider != null;
            }
        }

        // 1�b�Ԉړ�
        async UniTask MoveAsync(Vector2Int input, CancellationToken token)
        {
            _body.forward = new Vector3(input.x, 0, input.y);
            for (int i = 0; i <= 60; i++)
            {
                _rigidbody.velocity = new Vector3(input.x, _rigidbody.velocity.y, input.y);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }
        }

        // ���E�ړ��̑��x��0�ɂ���
        void Idle()
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }

        // ���͂̏󋵂𒲂ׂ�
        void CheckSurroundings()
        {
            // �X�e�[�W�̉��ɗ����Ă��邩����
            _gameState.OnStageBorder = Physics.Raycast(_eye.position, _eye.forward, 
                _stageBorderRaycastLength, Const.StageBorderLayer);
            // ���̎�O�ɗ����Ă��邩����
            _gameState.OnHoleFront = Physics.Raycast(_eye.position, _eye.forward + Vector3.down, 
                _holeFrontRaycastLength, Const.HoleRangeLayer);
            // �ڂ̑O�ɒi�������邩����
            _gameState.OnStepFront = Physics.Raycast(_kness.position, _eye.forward,
                _stepFrontRaycastLength, Const.FootingLayer);

            Debug.Log("�X�e�[�W�̒[:" + _gameState.OnStageBorder);
        }

        /// <summary>
        /// �����I�Ɉړ�������
        /// </summary>
        public void Teleport(Vector3 position)
        {
            _transform.position = position;
        }

        void OnDrawGizmos()
        {
            DrawGroundingLinecast();
            DrawStageBorderRaycast();
            DrawHoleFrontRaycast();
            DrawStepFrontRaycast();
        }

        // �ڒn������M�Y���ɕ`��
        void DrawGroundingLinecast()
        {
            if (_transform == null) return;

            Line(Vector3.left * _groundingRadius);
            Line(Vector3.right * _groundingRadius);

            void Line(Vector3 side)
            {
                Vector3 center = _transform.position + side + _groundingOffset;
                Vector3 halfHeight = _transform.up * (_groundingHeight / 2);
                Gizmos.DrawLine(center + halfHeight, center - halfHeight);
            }
        }

        // �X�e�[�W�[����̃��C�L���X�g���M�Y���ɕ`��
        void DrawStageBorderRaycast()
        {
            if (_eye == null) return;

            Gizmos.DrawRay(_eye.position, _eye.forward * _stageBorderRaycastLength);
        }

        // ��O�Ɍ������锻��̃��C�L���X�g���M�Y���ɕ`��
        void DrawHoleFrontRaycast()
        {
            if (_eye == null) return;

            Vector3 dir = (_eye.forward + Vector3.down).normalized;
            Gizmos.DrawRay(_eye.position, dir * _stageBorderRaycastLength);
        }

        // �i������̃��C�L���X�g���M�Y���ɕ`��
        void DrawStepFrontRaycast()
        {
            if (_kness == null) return;

            Gizmos.DrawRay(_kness.position, _kness.forward * _stepFrontRaycastLength);
        }
    }
}
