using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;

namespace PSB.Game
{
    public class InGameEntryPoint : MonoBehaviour
    {
        GameState _gameState;
        DungeonManager _dungeonManager;
        UiManager _uiManager;

        [Inject]
        void Construct(GameState gameState, DungeonManager dungeonManager, UiManager uiManager)
        {
            _gameState = gameState;
            _dungeonManager = dungeonManager;
            _uiManager = uiManager;
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            await _dungeonManager.BuildAsync(token);
            // �v���C���[�̑�����󂯕t���鏀������
            MessageBroker.Default.Publish(new InGameReadyMessage());
            // �󔠂����܂ő҂�
            await UniTask.WaitUntil(() => _gameState.IsGetTreasure);
            // �����ɗ��܂ő҂�
            await UniTask.WaitUntil(() => _gameState.IsStandingEntrance);

            // �Q�[���N���A�̉��o
            await _uiManager.GameClearAnimationAsync(token);
        }

        // �M�Y���ɕ`�悷��C�x���g�֐����g�p���邽�߂�VContainer�̃G���g���|�C���g���g�p����MonoBehavior���g���B
        // ��p�̃N���X�����Ηǂ��������܂ł���K�v���B
        void OnDrawGizmos()
        {
            if(_dungeonManager != null) _dungeonManager.DrawOnGizmos();
        }
    }
}
