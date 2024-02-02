using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFC�A���S���Y������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���B
using PSB.Game.SAW; // ���ȉ���E�H�[�N�A���S���Y���͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���B

namespace PSB.Game
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] TileBuilder _tileBuilder;
        [Header("�_���W���������ݒ�")]
        [SerializeField] int _size = 20;
        [SerializeField] float _cellSize = 1;
        [Header("WFC�ݒ�")]
        [Min(1)]
        [SerializeField] uint _wfcSeed = 1;
        [SerializeField] bool _wfcRandomSeed;
        [Header("���ȉ���E�H�[�N�ݒ�")]
        [Min(1)]
        [SerializeField] uint _walkSeed = 1;
        [SerializeField] bool _walkRandomSeed;
        [Header("�X�V���x")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Dungeon _dungeon;
        SelfAvoidingWalk _walk;

        void Awake()
        {
            // �^�C���𐶐����Ă���Ƀ_���W�����̃O���b�h�����킹��B
            // 2�̃^�C���̊Ԃ�1�̃Z�������݂���̂ŁA�T�C�Y���̂܂܂���1����Ȃ��Ȃ��Ă��܂��B
            _dungeon = new(_size - 1, _size - 1, _cellSize);
            uint seed = _walkRandomSeed ? (uint)Random.Range(1, uint.MaxValue) : _walkSeed;
            _walk = new(_size - 1, _size - 1, _cellSize, seed);
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            //await BuildDungeonBaseAsync(token);
            await CreatePathAsync(token);



            // ��O���o�邩�e�X�g�p
            //await UniTask.Yield();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(
            //    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            // �_���W���������Ɏg���N���X�͊��������烁������������Ă��ǂ���������Ȃ�
        }

        // �g���֐��̕���A���S���Y���Ńx�[�X�ƂȂ�_���W���������
        async UniTask BuildDungeonBaseAsync(CancellationToken token)
        {
            // �V�[�h�l�����߂Đ���
            uint seed = _wfcRandomSeed ? (uint)Random.Range(1, uint.MaxValue) : _wfcSeed;
            Logic logic = new(_size, _size, seed);

            for (int i = 0; i < _size * _size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        // ���ȉ���E�H�[�N�A���S���Y���ŃX�^�[�g����S�[���ֈ�{�������
        async UniTask CreatePathAsync(CancellationToken token)
        {
            for (int i = 0; i < (_size - 1) * (_size - 1); i++)
            {
                _walk.Step();
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
            if (_walk != null) _walk.DrawGridOnGizmos();
        }
    }
}
