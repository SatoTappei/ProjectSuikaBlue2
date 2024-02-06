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
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] TileBuilder _tileBuilder;
        [SerializeField] EntityCreator _entityCreator;
        [Header("�f�o�b�O�p:�_���W�����݂̂œ���")]
        [SerializeField] bool _debug = true;

        GameState _gameState;
        Dungeon _dungeon;
        SelfAvoidingWalk _walk;
        DungeonParameterSettings _settings;
        
        [Inject]
        void Construct(GameState gameState, DungeonParameterSettings settings)
        {
            _gameState = gameState;
            _settings = settings;
        }

        void Awake()
        {
            Init();      
        }

        void Start()
        {
            if (_debug)
            {
                BuildAsync(this.GetCancellationTokenOnDestroy()).Forget();
                Debug.LogWarning("�_���W�����݂̂Ŏ��s��");
            }
        }

        void Init()
        {
            // �^�C���𐶐����Ă���Ƀ_���W�����̃O���b�h�����킹��B
            // 2�̃^�C���̊Ԃ�1�̃Z�������݂���̂ŁA�T�C�Y���̂܂܂���1����Ȃ��Ȃ��Ă��܂��B
            int h = _settings.Size - 1;
            int w = _settings.Size - 1;

            _dungeon = new(h, w, _settings.CellSize);

            uint seed = _settings.WalkRandomSeed ? Utility.RandomSeed() : _settings.WalkSeed;
            _walk = new(h, w, seed);
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
            // �_���W���������Ɏg���N���X�͊��������烁������������Ă��ǂ���������Ȃ�
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
            _walk.SetStartPoint(_settings.Entry.y, _settings.Entry.x);

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
                    Physics.RaycastNonAlloc(origin, dir, hit, dist);
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
                foreach(Vector2Int neighbour in Direction())
                {
                    int nx = cell.Index.x + neighbour.x;
                    int ny = cell.Index.y + neighbour.y;              
                    if (!Utility.CheckInLength(_dungeon.Grid, ny, nx)) continue;

                    // 1�̃Z����4�̃^�C�����ꂼ��̈ꕔ����\������Ă��鐫����
                    // �^�C���̎�ނ��炻�̃Z�����㉺���E�ɐi�߂邩�̔��肪���������߁A���C�L���X�g�Ŕ��肷��B
                    Vector3 origin = cell.Position + _settings.CellRayOffset;
                    Vector3 dir = new Vector3(neighbour.x, 0, neighbour.y);
                    float dist = (cell.Position - _dungeon.Grid[ny, nx].Position).magnitude;
                    if (Physics.Raycast(origin, dir, dist)) continue;

                    // �ׂ̃Z���Ƀ��C�L���X�g���q�b�g���Ȃ������ꍇ�͐ڑ�����Ă���B
                    _dungeon.Connect(cell.Index.x, cell.Index.y, nx, ny);
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
            // �o�H�̒[�ɃX�^�[�g�ƃS�[��
            //Vector3 s = new Vector3(_entry)
            //_entityCreator.Location(LocationKey.Start,)
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
        /// �ړ��\�����`�F�b�N���A�\�ȏꍇ�͈ړ����Ԃ�
        /// </summary>
        public bool TryGetMovablePosition(Vector2Int origin, Vector2Int direction, out Vector3 position)
        {
            // �㉺���E�̒P�ʃx�N�g���ɐ������Ă��Ȃ��̂�2�Z���ȏ�̈ړ����\
            Vector2Int n = origin + direction;
            
            if (_dungeon.IsConnected(origin.x, origin.y, n.x, n.y))
            {
                position = _dungeon.Grid[n.y, n.x].Position;
                return true;
            }

            position = _dungeon.Grid[origin.y, origin.x].Position;
            return false;
        }

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
            if (_walk != null) _walk.DrawGridOnGizmos(_settings.CellSize);
        }
    }
}