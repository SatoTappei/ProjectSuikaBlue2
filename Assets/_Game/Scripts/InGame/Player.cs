using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;

namespace PSB.Game
{
    public class Player : MonoBehaviour, ICharacter
    {
        GameState _gameState;
        PlayerParameterSettings _settings;
        DungeonManager _dungeonManager;
        Vector2Int _currentIndex;
        Direction _forward;

        [Inject]
        void Construct(GameState gameState, PlayerParameterSettings settings, DungeonManager dungeonManager)
        {
            _gameState = gameState;
            _settings = settings;
            _dungeonManager = dungeonManager;
        }

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // �����蔻��p�Ɏg���p�ɐݒ�
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.mass = 0;
            rb.angularDrag = 0;

            // �_���W�����̓����������ʒu
            transform.position = _dungeonManager.GetStartCell().Position;
            transform.Translate(_settings.GroundOffset);
            _currentIndex = _dungeonManager.GetStartCell().Index;
            _gameState.PlayerIndex = _currentIndex;

            // �O���b�h��ł̈ʒu
            _dungeonManager.SetCharacter(CharacterKey.Player, _currentIndex, this);

            // ��]��0�̏�Ԃ͖k����
            transform.rotation = Quaternion.identity;
            _forward = Direction.North;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // ���������̃t���O�����܂ő��삳���Ȃ�
            await UniTask.WaitUntil(() => _gameState.IsInGameReady, cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                // ���͂𒲂ׂ�
                SearchAround();

                // ���͂̃��b�Z�[�W�����ł���܂őҋ@
                KeyInputMessage msg = await MessageAwaiter.ReceiveAsync<KeyInputMessage>(token);

                // �Q�[���̃N���A�����𖞂����Ă����ꍇ�͒e��
                if (_gameState.IsInGameClear) continue;

                // �ړ��������͉�]
                if (msg.IsMoveKey(out KeyCode moveKey))
                {
                    await MoveAsync(moveKey, token);
                }
                else if (msg.IsRotateKey(out KeyCode rotKey))
                {
                    await RotateAsync(rotKey, token);
                }

                // �s������
                ActionResult();

                await UniTask.Yield(token);
            }
        }

        // �ړ�
        async UniTask MoveAsync(KeyCode key, CancellationToken token)
        {
            // �O��ړ��̃L�[������A�����ɉ������ׂ̃Z�����w���������擾
            if (!key.TryGetFrontAndBackIndex(_forward, out Vector2Int direction)) return;
            // �ړ��悪���邩�`�F�b�N
            Vector2Int neighbour = _currentIndex + direction;
            if (!_dungeonManager.IsConnected(_currentIndex, neighbour)) return;

            // �����Đ�
            AudioPlayer.PlayLoop(this, _settings.WalkSeLoop, _settings.WalkSeDelay, 
                AudioKey.WalkStepSE, AudioPlayer.PlayMode.SE);

            // �����̃I�t�Z�b�g�𑫂����ړ��ʂ��ړ�
            Vector3 move = _dungeonManager.GetCell(neighbour).Position + _settings.GroundOffset - transform.position;
            float speed = _settings.MoveSpeed;
            await transform.MoveAsync(move, speed, token);

            // ���݂̃O���b�h��ł̈ʒu���폜
            _dungeonManager.SetCharacter(CharacterKey.Dummy, _currentIndex, null);

            _currentIndex = neighbour;
            _gameState.PlayerIndex = _currentIndex;

            // �O���b�h��ł̈ʒu���X�V
            _dungeonManager.SetCharacter(CharacterKey.Player, _currentIndex, this);
        }

        // ��]
        // ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g���̂���]����
        async UniTask RotateAsync(KeyCode key, CancellationToken token)
        {
            float rot = key.To90DegreeRotateAngle();
            float speed = _settings.RotateSpeed;
            await transform.RotateAsync(rot, speed, token);
            
            _forward = key.ToTurnedDirection(_forward);
        }

        // �s���������ʂǂ��Ȃ�����
        void ActionResult()
        {
            IReadOnlyCell cell = _dungeonManager.GetCell(_currentIndex);

            // ���݈ʒu���󔠂Ȃ�l��
            if (cell.LocationKey == LocationKey.Chest)
            {
                _gameState.IsGetTreasure = true;
            }

            // ���ݒn�ɉ������C�x���g����
            cell.Location?.Action();
        }

        // AI�����݂̏󋵂�c�����邽�߁A���͂𒲂ׂ�GameState�ɏ�������
        void SearchAround()
        {
            // ���݂̃Z���̈ʒu����Ɋ�̍�������
            Vector3 p = _dungeonManager.GetCell(_currentIndex).Position;
            p.y = _settings.EyeHeight;

            // �O�㍶�E�̐i�߂鋗�����`�F�b�N
            _gameState.ForwardEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Up).ToIndex());
            _gameState.BackEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Down).ToIndex());
            _gameState.LeftEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Left).ToIndex());
            _gameState.RightEvaluate = CheckDistance(_forward.TurnedDirection(Arrow.Right).ToIndex());

            // ���̕����ɂǂꂾ���i�߂邩
            int CheckDistance(Vector2Int index)
            {
                for (int i = 1; ; i++)
                {
                    if (!_dungeonManager.IsConnected(_currentIndex + index * i, _currentIndex + index * (i - 1)))
                    {
                        return i - 1;
                    }
                }
            }
        }

        // �_���[�W
        void IDamageReceiver.Damage()
        {
            _gameState.LastDamagedTime = Time.time;

            AudioPlayer.Play(AudioKey.KickDamageSE, AudioPlayer.PlayMode.SE);
            CameraController.Shake();
        }
    }
}