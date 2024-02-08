using System.Collections;
using System.Collections.Generic;
using System;
using System.Buffers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFC�A���S���Y������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���B
using PSB.Game.SAW; // ���ȉ�������_���E�H�[�N�A���S���Y���͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���B
using VContainer;

namespace PSB.Game
{
    public class DungeonManager
    {
        GameState _gameState;
        Dungeon _dungeon;
        SelfAvoidingWalk _walk;
        DungeonParameterSettings _settings;
        TileBuilder _tileBuilder;
        EntityCreator _entityCreator;

        public DungeonManager(GameState gameState, DungeonParameterSettings settings,
            TileBuilder tileBuilder, EntityCreator entityCreator)
        {
            _gameState = gameState;
            _settings = settings;
            _tileBuilder = tileBuilder;
            _entityCreator = entityCreator;

            // �^�C���𐶐����Ă���Ƀ_���W�����̃O���b�h�����킹��B
            // 2�̃^�C���̊Ԃ�1�̃Z�������݂���̂ŁA�T�C�Y���̂܂܂���1����Ȃ��Ȃ��Ă��܂��B
            Init(_settings.Size - 1);
        }

        void Init(int size)
        {
            // �_���W�����̏��̃N���X
            _dungeon = new(size, size, _settings.CellSize);
            // �X�^�[�g����S�[���܂ł̌o�H�����A���S���Y��
            uint seed = _settings.WalkRandomSeed ? Utility.RandomSeed() : _settings.WalkSeed;
            _walk = new(size, size, seed);
        }

        /// <summary>
        /// �_���W�����̐���
        /// </summary>
        public async UniTask BuildAsync(CancellationToken token)
        {
            await BuildDungeonBaseAsync(token);
            await CreatePathAsync(token);
            RemoveWallOnPath();
            CheckNeighbourCell();
            CreateLocation();

            // TODO:�_���W���������Ɏg���N���X�͊��������烁������������Ă��ǂ���������Ȃ�
        }

        // �x�[�X�ƂȂ�_���W���������
        async UniTask BuildDungeonBaseAsync(CancellationToken token)
        {
            // �V�[�h�l�����߂Đ���
            uint seed = _settings.WfcRandomSeed ? Utility.RandomSeed() : _settings.WfcSeed;
            int size = _settings.Size;
            Logic logic = new(size, size, seed);

            for (int i = 0; i < size * size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_settings.StepSpeed, cancellationToken: token);
            }
        }

        // �X�^�[�g����S�[���ֈ�{�������
        async UniTask CreatePathAsync(CancellationToken token)
        {
            // �C�ӂ̈ʒu����X�^�[�g
            _walk.SetStartPoint(_settings.Entry);
            _gameState.StartIndex = _settings.Entry;

            for (int i = 0; i < _walk.GridLength; i++)
            {
                // ����ȏ�i�߂Ȃ��Ȃ����ꍇ�͑ł��؂�
                if (!_walk.Step()) break;
                await UniTask.WaitForSeconds(_settings.StepSpeed, cancellationToken: token);
            }
        }

        // �X�^�[�g����S�[���܂ł̌o�H�ɂ���ǂ��폜����
        void RemoveWallOnPath()
        {
            RaycastHit[] hit = ArrayPool<RaycastHit>.Shared.Rent(_settings.CellRayHitMax);
            Vector3 prev = default;
            bool first = true;
            // �o�H�̊e�Z�����m�����C�L���X�g
            foreach(SelfAvoidingWalk.Cell c in _walk.Path)
            {
                // �A���S���Y���̃N���X�͎��ۂ̍��W��ێ����Ă��Ȃ��̂ō��W�ɕϊ�
                Vector3 current = Dungeon.IndexToPosition(c.Index, _settings.CellSize);

                if (!first)
                {
                    Vector3 origin = prev + _settings.CellRayOffset;
                    Vector3 dir = (current - prev).normalized;
                    float dist = (current - prev).magnitude;
                    Physics.RaycastNonAlloc(origin, dir, hit, dist, Const.DungeonLayer);
                    foreach (RaycastHit h in hit)
                    {
                        if (h.collider == null) break;
                        if (!h.collider.TryGetComponent(out IDungeonParts parts)) continue;

                        // �Z���̒��S����Ƃ����ʒu���m�����C�L���X�g���Ă���̂�
                        // 2�ȏ�̎q�I�u�W�F�N�g�����������ꍇ�͏d�Ȃ荇���p�ɃR���C�_�[��ݒu����K�v������B
                        parts.Remove();
                    }
                }

                prev = current;
                first = false;
            }

            Array.Clear(hit, 0, hit.Length);
            ArrayPool<RaycastHit>.Shared.Return(hit);
        }

        // �S�ẴZ�����㉺���E�̗אڂ���Z���ɐi�߂邩�𔻒�
        void CheckNeighbourCell()
        {
            foreach (IReadOnlyCell cell in _dungeon.Grid)
            {
                foreach(Vector2Int d in Direction())
                {
                    Vector2Int neighbour = cell.Index + d;            
                    if (!Utility.CheckInLength(_dungeon.Grid, neighbour)) continue;

                    // 1�̃Z����4�̃^�C�����ꂼ��̈ꕔ����\������Ă��鐫����
                    // �^�C���̎�ނ��炻�̃Z�����㉺���E�ɐi�߂邩�̔��肪���������߁A���C�L���X�g�Ŕ��肷��B
                    Vector3 origin = cell.Position + _settings.CellRayOffset;
                    Vector3 dir = new Vector3(d.x, 0, d.y);
                    float dist = (cell.Position - _dungeon.Position(neighbour)).magnitude;
                    if (Physics.Raycast(origin, dir, dist, Const.DungeonLayer)) continue;

                    // �ׂ̃Z���Ƀ��C�L���X�g���q�b�g���Ȃ������ꍇ�͐ڑ�����Ă���B
                    _dungeon.Connect(cell.Index, neighbour);
                }
            }

            IEnumerable<Vector2Int> Direction()
            {
                yield return Vector2Int.up;
                yield return Vector2Int.down;
                yield return Vector2Int.left;
                yield return Vector2Int.right;
            }
        }

        // �ꏊ�̐���
        void CreateLocation()
        {
            _entityCreator.Location(LocationKey.Start, _dungeon.Position(_settings.Entry));
            // �o�H�̌��݈ʒu���o�H�̖��[�܂�S�[���ɂȂ��Ă���
            _entityCreator.Location(LocationKey.Chest, _dungeon.Position(_walk.Current));

            _dungeon.SetLocation(_settings.Entry, LocationKey.Start);
            _dungeon.SetLocation(_walk.Current, LocationKey.Chest);
        }

        /// <summary>
        /// �_���W�����̃X�^�[�g�ʒu��Ԃ��B
        /// ���̈ʒu����K���S�[���ɂ��ǂ蒅�������o����B
        /// </summary>
        public Vector3 StartPosition() => Dungeon.IndexToPosition(_settings.Entry, _settings.CellSize);

        /// <summary>
        /// �_���W�����̃X�^�[�g�ʒu�̃C���f�b�N�X��Ԃ��B
        /// ���̈ʒu����K���S�[���ɂ��ǂ蒅�������o����B
        /// </summary>
        public Vector2Int StartIndex() => _settings.Entry;

        /// <summary>
        /// �Z�����ǂ̂悤�ȏꏊ�Ȃ̂������߂�L�[���Z�b�g
        /// </summary>
        public void SetLocation(Vector2Int index, LocationKey key) => _dungeon.SetLocation(index, key);

        /// <summary>
        /// �Z���ɂ���A�C�e���𔻒肷�邽�߂̃L�[���Z�b�g
        /// </summary>
        public void SetItem(Vector2Int index, ItemKey key) => _dungeon.SetItem(index, key);

        /// <summary>
        /// �Z���ɂ���L�����N�^�[�𔻒肷�邽�߂̃L�[��Ԃ�
        /// </summary>
        public void SetCharacter(Vector2Int index, CharacterKey key) => _dungeon.SetCharacter(index, key);

        /// <summary>
        /// �Z�����ǂ̂悤�ȏꏊ�Ȃ̂������߂�L�[��Ԃ�
        /// </summary>
        public LocationKey GetLocation(Vector2Int index) => _dungeon.GetLocation(index);

        /// <summary>
        /// �Z���ɂ���A�C�e���𔻒肷�邽�߂̃L�[��Ԃ�
        /// </summary>
        public ItemKey GetItem(Vector2Int index) => _dungeon.GetItem(index);

        /// <summary>
        /// �Z���ɂ���L�����N�^�[�𔻒肷�邽�߂̃L�[��Ԃ�
        /// </summary>
        public CharacterKey GetCharacter(Vector2Int index) => _dungeon.GetCharacter(index);

        /// <summary>
        /// �ړ��\�����`�F�b�N���A�\�ȏꍇ�͈ړ����Ԃ�
        /// </summary>
        public bool TryGetMovablePosition(Vector2Int origin, Vector2Int direction, out Vector3 position)
        {
            // �㉺���E�̒P�ʃx�N�g���ɐ������Ă��Ȃ��̂�2�Z���ȏ�̈ړ����\
            Vector2Int n = origin + direction;
            
            if (_dungeon.IsConnected(origin, n))
            {
                position = _dungeon.Position(n);
                return true;
            }

            position = _dungeon.Position(origin);
            return false;
        }

        /// <summary>
        /// �_���W�����̏����M�Y���ɕ`��
        /// </summary>
        public void DrawOnGizmos()
        {
            _dungeon.DrawGridOnGizmos();
            _walk.DrawGridOnGizmos(_settings.CellSize);
        }
    }
}