using System.Collections;
using System.Collections.Generic;
using System;
using System.Buffers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using PSB.Game.WFC; // WFC�A���S���Y������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���B
using PSB.Game.SAW; // ���ȉ�������_���E�H�[�N�A���S���Y���͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���B

namespace PSB.Game
{
    /// <summary>
    /// �_���W�����Ƃ����ɐݒu�����I�u�W�F�N�g�̐������s���B
    /// �_���W�����̊e�Z���̒l�̓ǂݏ����A�o�H�T���@�\���O���ɒ񋟂���B
    /// �K�v�ȃN���X��DI���Ďg���B
    /// </summary>
    public class DungeonManager
    {
        GameState _gameState;
        Dungeon _dungeon;
        AStar _aStar;
        SelfAvoidingWalk _walk;
        DungeonParameterSettings _settings;
        TileBuilder _tileBuilder;
        EntityCreator _entityCreator;

        // A*�ŋ��߂��o�H���M�Y���ɕ`�悷��p�r
        List<IReadOnlyCell> _pathCopy = new();

        /// <summary>
        /// �_���W������1�ӂ̒��a�B
        /// </summary>
        public float Diameter => _settings.CellSize * _settings.Size;
        /// <summary>
        /// �_���W������1�ӂ̃Z���̐��B
        /// </summary>
        public int Size => _settings.Size - 1;

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
            // �o�H�T��
            _aStar = new(_dungeon.Grid);
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
                    Vector3 dir = MyMath.Normalize(current - prev);
                    float dist = MyMath.Magnitude(current - prev);
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
                    float dist = (cell.Position - _dungeon[neighbour].Position).magnitude;
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
            CreateAndSet(LocationKey.Start, _settings.Entry);
            // �o�H�̌��݈ʒu���o�H�̖��[�܂�S�[���ɂȂ��Ă���
            CreateAndSet(LocationKey.Chest, _walk.Current);

            void CreateAndSet(LocationKey key, Vector2Int index)
            {
                Vector3 p = _dungeon[index].Position;
                _dungeon.SetLocation(key, index, _entityCreator.Location(key, p));
            }
        }

        /// <summary>
        /// �_���W�����̃X�^�[�g�ʒu�̃Z����Ԃ��B
        /// ���̈ʒu����K���S�[���ɂ��ǂ蒅�������o����B
        /// </summary>
        public IReadOnlyCell GetStartCell() => _dungeon[_settings.Entry];

        /// <summary>
        /// �Z����Ԃ��B
        /// </summary>e
        public IReadOnlyCell GetCell(Vector2Int index) => _dungeon[index];

        /// <summary>
        /// �Z���ɃL�����N�^�[���Z�b�g�B
        /// </summary>
        public void SetCharacter(CharacterKey key, Vector2Int index, ICharacter character)
        {
            _dungeon.SetCharacter(key, index, character);
        }

        /// <summary>
        /// 2�̃Z�����ڑ�����Ă��邩��Ԃ��B
        /// </summary>
        public bool IsConnected(Vector2Int a, Vector2Int b) => _dungeon.IsConnected(a, b);

        /// <summary>
        /// 2�_�Ԃ̌o�H�T��
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> Pathfinding(Vector2Int a, Vector2Int b)
        {
            List<IReadOnlyCell> path = _aStar.Pathfinding(a, b);
            // �M�Y���ɕ`�悷��p�ɃR�s�[���Ă���
            _pathCopy = path;

            return path;
        }

        /// <summary>
        /// �_���W�����̏����M�Y���ɕ`��
        /// </summary>
        public void DrawOnGizmos()
        {
            _dungeon.DrawGridOnGizmos();
            _walk.DrawGridOnGizmos(_settings.CellSize);
            DrawPathOnGizmos();
            DrawCharacterOnGizmos();
        }

        // �Ō�ɋ��߂��o�H��`��
        void DrawPathOnGizmos()
        {
            if (_pathCopy == null) return;

            Gizmos.color = Color.yellow;
            foreach (IReadOnlyCell v in _pathCopy) Gizmos.DrawCube(v.Position, Vector3.one);
        }

        // �L�����N�^�[�̈ʒu��`��
        void DrawCharacterOnGizmos()
        {
            foreach (var v in _dungeon.Grid)
            {
                if (v.CharacterKey == CharacterKey.Player)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(v.Position, 1);
                }
                else if (v.CharacterKey == CharacterKey.Enemy)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(v.Position, 1);
                }
            }
        }
    }
}