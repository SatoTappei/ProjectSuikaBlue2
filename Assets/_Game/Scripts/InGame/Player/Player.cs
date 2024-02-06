using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using System.Globalization;
using YamlDotNet.Serialization;

namespace PSB.Game
{
    // ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g���ړ��Ɖ�]�ǂ�����s��
    public class Player : MonoBehaviour
    {
        [SerializeField] DungeonManager _dungeonManager;

        GameState _gameState;
        PlayerParameterSettings _settings;
        Vector2Int _currentIndex;
        Direction _forward;

        [Inject]
        void Construct(GameState gameState, PlayerParameterSettings settings)
        {
            _gameState = gameState;
            _settings = settings;
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
            // �_���W�����̓����������ʒu
            transform.position = _dungeonManager.StartPosition();
            transform.Translate(_settings.GroundOffset);
            _currentIndex = _dungeonManager.StartIndex();

            // ��]��0�̏�Ԃ͖k����
            _forward = Direction.North;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // �L�����Z�����ɍs�����X�L�b�v�����
                using (CancellationTokenSource skipTokenSource = new())
                {
                    // ���͂̃��b�Z�[�W�����ł���܂őҋ@
                    KeyInputMessage msg = await MessageAwaiter.ReceiveAsync<KeyInputMessage>(token);

                    // �ړ��������͉�]
                    if (msg.IsMoveKey(out KeyCode moveKey))
                    {
                        await MoveAsync(moveKey, skipTokenSource.Token);
                    }
                    else if (msg.IsRotateKey(out KeyCode rotKey))
                    {
                        await RotateAsync(rotKey, skipTokenSource.Token);
                    }
                }
            }
        }

        // �ړ�
        async UniTask MoveAsync(KeyCode key, CancellationToken skipToken)
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
            await transform.MoveAsync(move, speed, skipToken);

            _currentIndex += neighbour;
        }

        // ��]
        async UniTask RotateAsync(KeyCode key, CancellationToken skipToken)
        {
            float rot = key.To90DegreeRotateAngle();
            float speed = _settings.RotateSpeed;
            await transform.RotateAsync(rot, speed, skipToken);
            
            _forward = key.ToTurnedDirection(_forward);
        }
    }
}