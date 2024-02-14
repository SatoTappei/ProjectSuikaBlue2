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
            FlowAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // �Q�[���J�n���瑀��I���܂ł̈�A�̗���
        async UniTaskVoid FlowAsync(CancellationToken token)
        {
            Init();

            // ���������̃��b�Z�[�W����M����܂ő��삳���Ȃ�
            await MessageAwaiter.ReceiveAsync<InGameReadyMessage>(token);

            await UpdateAsync(token);
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
            while (!token.IsCancellationRequested)
            {
                // ���͂̃��b�Z�[�W�����ł���܂őҋ@
                KeyInputMessage msg = await MessageAwaiter.ReceiveAsync<KeyInputMessage>(token);

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

        // �_���[�W
        void IDamageReceiver.Damage()
        {
            // �����ŏ�������̂ł͂Ȃ��A�_���[�W�̓��e���L���[�C���O���ĔC�ӂ̃^�C�~���O�ŏ������������ǂ�
            Debug.Log(name + "���_���[�W���󂯂�");
            CameraController.Shake();
        }
    }
}