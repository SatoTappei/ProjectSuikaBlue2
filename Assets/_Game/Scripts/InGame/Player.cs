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
        [Header("視界のレイキャストの始点")]
        [SerializeField] Transform _eye;
        [Header("段差判定のレイキャストの始点")]
        [SerializeField] Transform _kness;
        [Header("向いている方向の基準")]
        [SerializeField] Transform _body;
        [Header("接地判定用のレイキャストの設定")]
        [SerializeField] float _groundingRadius = 0.5f;
        [SerializeField] float _groundingHeight = 0.1f;
        [SerializeField] Vector3 _groundingOffset;
        [Header("ジャンプの設定")]
        [SerializeField] float _jumpInterval = 0.5f;
        [SerializeField] float _verticalJumpPower = 5.0f;
        [SerializeField] float _horizontalJumpPowr = 3.0f;
        [Header("周囲判定用のレイキャストの設定")]
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
                // メッセージが飛んでくるまで待機
                PlayerControlMessage msg = await MessageBroker.Default
                    .Receive<PlayerControlMessage>().ToUniTask(useFirstValue: true, token);

                // どの移動キーが入力されたか
                Vector2Int input = default;
                if (msg.KeyDownA) { input.x--; }
                if (msg.KeyDownD) { input.x++; }

                Rotation(input);

                // ジャンプ or 移動
                if (msg.KeyDownSpace) { await JumpAsync(input, token); }
                else { await MoveAsync(input, token); }

                Idle();
                CheckSurroundings();

                await UniTask.Yield(token);
            }
        }

        // 方向に向く
        void Rotation(Vector2Int input)
        {
            _body.forward = new Vector3(input.x, 0, input.y);

            if (input == Vector2Int.left) _gameState.Forward = Forward.Left;
            else if (input == Vector2Int.right) _gameState.Forward = Forward.Right;
        }

        // 方向に飛ぶ
        async UniTask JumpAsync(Vector2Int input, CancellationToken token)
        {
            Vector3 force = Vector3.up * _verticalJumpPower + Vector3.right * input.x * _horizontalJumpPowr;
            _rigidbody.AddForce(force, ForceMode.Impulse);

            // ジャンプ中に横方向に力を加え続けることで、段差に引っかかっても乗り越えることが出来る。
            for (float f = 0; f < _jumpInterval; f += Time.fixedDeltaTime)
            {
                Vector3 velo = _rigidbody.velocity;
                velo.x = input.x * _horizontalJumpPowr;
                _rigidbody.velocity = velo;
                // ジャンプした次のフレームではレイキャストが地面から離れないので、判定までのクールタイムを設ける。
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: token);
            }
 
            await UniTask.WaitUntil(IsGrounding, cancellationToken: token);
        }

        // 接地判定
        bool IsGrounding()
        {
            // Z軸方向には動かないので2箇所判定すれば大丈夫
            return Linecast(Vector3.left * _groundingRadius) ||
                   Linecast(Vector3.right * _groundingRadius);

            // 中心位置から引数だけずらした位置に縦方向の線キャスト
            bool Linecast(Vector3 side)
            {
                Vector3 center = _transform.position + side + _groundingOffset;
                Vector3 halfHeight = _transform.up * (_groundingHeight / 2);
                Physics.Linecast(center + halfHeight, center - halfHeight, out RaycastHit hit, Const.FootingLayer);
                return hit.collider != null;
            }
        }

        // 1秒間移動
        async UniTask MoveAsync(Vector2Int input, CancellationToken token)
        {
            _body.forward = new Vector3(input.x, 0, input.y);
            for (int i = 0; i <= 60; i++)
            {
                _rigidbody.velocity = new Vector3(input.x, _rigidbody.velocity.y, input.y);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }
        }

        // 左右移動の速度を0にする
        void Idle()
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }

        // 周囲の状況を調べる
        void CheckSurroundings()
        {
            // ステージの縁に立っているか判定
            _gameState.OnStageBorder = Physics.Raycast(_eye.position, _eye.forward, 
                _stageBorderRaycastLength, Const.StageBorderLayer);
            // 穴の手前に立っているか判定
            _gameState.OnHoleFront = Physics.Raycast(_eye.position, _eye.forward + Vector3.down, 
                _holeFrontRaycastLength, Const.HoleRangeLayer);
            // 目の前に段差があるか判定
            _gameState.OnStepFront = Physics.Raycast(_kness.position, _eye.forward,
                _stepFrontRaycastLength, Const.FootingLayer);

            Debug.Log("ステージの端:" + _gameState.OnStageBorder);
        }

        /// <summary>
        /// 強制的に移動させる
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

        // 接地判定をギズモに描画
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

        // ステージ端判定のレイキャストをギズモに描画
        void DrawStageBorderRaycast()
        {
            if (_eye == null) return;

            Gizmos.DrawRay(_eye.position, _eye.forward * _stageBorderRaycastLength);
        }

        // 手前に穴がある判定のレイキャストをギズモに描画
        void DrawHoleFrontRaycast()
        {
            if (_eye == null) return;

            Vector3 dir = (_eye.forward + Vector3.down).normalized;
            Gizmos.DrawRay(_eye.position, dir * _stageBorderRaycastLength);
        }

        // 段差判定のレイキャストをギズモに描画
        void DrawStepFrontRaycast()
        {
            if (_kness == null) return;

            Gizmos.DrawRay(_kness.position, _kness.forward * _stepFrontRaycastLength);
        }
    }
}
