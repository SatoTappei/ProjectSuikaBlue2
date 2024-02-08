using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using System.Globalization;
using YamlDotNet.Serialization;
using Unity.VisualScripting;

namespace PSB.Game
{
    // ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g���ړ��Ɖ�]�ǂ�����s��
    public class Player : MonoBehaviour
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
            transform.position = _dungeonManager.StartPosition();
            transform.Translate(_settings.GroundOffset);
            _currentIndex = _dungeonManager.StartIndex();
            _gameState.CurrentIndex = _currentIndex;

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
            if (!key.TryGetFrontAndBackIndexDirection(_forward, out Vector2Int neighbour)) return;
            // �ړ��悪���邩�`�F�b�N
            if (!_dungeonManager.TryGetMovablePosition(_currentIndex, neighbour, out Vector3 target)) return;

            // �����Đ�
            AudioPlayer.PlayLoop(this, _settings.WalkSeLoop, _settings.WalkSeDelay, 
                AudioKey.WalkStepSE, AudioPlayer.PlayMode.SE);

            // �����̃I�t�Z�b�g�𑫂����ړ��ʂ��ړ�
            Vector3 move = target + _settings.GroundOffset - transform.position;
            float speed = _settings.MoveSpeed;
            await transform.MoveAsync(move, speed, token);

            _currentIndex += neighbour;
            _gameState.CurrentIndex = _currentIndex;
        }

        // ��]
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
            // ���݈ʒu���󔠂Ȃ�l��
            LocationKey location = _dungeonManager.GetLocation(_currentIndex);
            Debug.Log("������" + location);
            if (location == LocationKey.Chest)
            {
                _gameState.IsGetTreasure = true;
            }
        }
    }
}