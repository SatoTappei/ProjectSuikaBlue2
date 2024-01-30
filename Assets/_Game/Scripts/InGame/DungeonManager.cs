using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFC�A���S���Y������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���

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
        [SerializeField] uint _seed = 1;
        [SerializeField] bool _randomSeed;
        [Header("�X�V���x")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Dungeon _dungeon;

        void Awake()
        {
            // �^�C���𐶐����Ă���Ƀ_���W�����̃O���b�h�����킹��B
            // 2�̃^�C���̊Ԃ�1�̃Z�������݂���̂ŁA�T�C�Y���̂܂܂���1����Ȃ��Ȃ��Ă��܂��B
            _dungeon = new(_size - 1, _size - 1, _cellSize);
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            _dungeon.CheckCell(1, 0);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            // �V�[�h�l�����߂Đ���
            uint seed = _randomSeed ? (uint)Random.Range(1, uint.MaxValue) : _seed;
            Logic logic = new(_size, _size, seed);

            for (int i = 0; i < _size * _size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
        }
    }
}
