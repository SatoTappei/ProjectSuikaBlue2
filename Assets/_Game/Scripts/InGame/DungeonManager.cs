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
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] TileBuilder _tileBuilder;
        [SerializeField] EntityCreator _entityCreator;
        [Header("�_���W���������ݒ�")]
        [SerializeField] int _size = 20;
        [SerializeField] float _cellSize = 1;
        [SerializeField] Vector2Int _entry;
        [Header("WFC�ݒ�")]
        [Min(1)]
        [SerializeField] uint _wfcSeed = 1;
        [SerializeField] bool _wfcRandomSeed;
        [Header("���ȉ���E�H�[�N�ݒ�")]
        [Min(1)]
        [SerializeField] uint _walkSeed = 1;
        [SerializeField] bool _walkRandomSeed;
        [Header("�Z���Ԃ̃��C�L���X�g�̐ݒ�")]
        [SerializeField] Vector3 _cellRayOffset = Vector3.up;
        [SerializeField] int _cellRayHitMax = 9;
        [Header("�X�V���x")]
        [Range(0.016f, 1.0f)]
        [SerializeField] float _stepSpeed = 0.016f;

        Dungeon _dungeon;
        SelfAvoidingWalk _walk;

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // �^�C���𐶐����Ă���Ƀ_���W�����̃O���b�h�����킹��B
            // 2�̃^�C���̊Ԃ�1�̃Z�������݂���̂ŁA�T�C�Y���̂܂܂���1����Ȃ��Ȃ��Ă��܂��B
            int h = _size - 1;
            int w = _size - 1;

            _dungeon = new(h, w, _cellSize);

            uint seed = _walkRandomSeed ? Utility.RandomSeed() : _walkSeed;
            _walk = new(h, w, seed);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
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
            uint seed = _wfcRandomSeed ? Utility.RandomSeed() : _wfcSeed;
            Logic logic = new(_size, _size, seed);

            for (int i = 0; i < _size * _size; i++)
            {
                _tileBuilder.Draw(logic.Step());
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        // �X�^�[�g����S�[���ֈ�{�������
        async UniTask CreatePathAsync(CancellationToken token)
        {
            // �C�ӂ̈ʒu����X�^�[�g
            _walk.SetStartPoint(_entry.y, _entry.x);

            for (int i = 0; i < _walk.GridLength; i++)
            {
                // ����ȏ�i�߂Ȃ��Ȃ����ꍇ�͑ł��؂�
                if (!_walk.Step()) break;
                await UniTask.WaitForSeconds(_stepSpeed, cancellationToken: token);
            }
        }

        // ������o�H���_���W�����ɔ��f������
        void CopyPathToDungeon()
        {
            foreach(SelfAvoidingWalk.Cell c in _walk.Path)
            {

            }
        }

        // �X�^�[�g����S�[���܂ł̌o�H�ɂ���ǂ��폜����
        void RemoveWallOnPath()
        {
            RaycastHit[] hit = ArrayPool<RaycastHit>.Shared.Rent(_cellRayHitMax);
            Vector3 prev = default;
            bool first = true;
            // �o�H�̊e�Z�����m�����C�L���X�g
            foreach(SelfAvoidingWalk.Cell c in _walk.Path)
            {
                // �A���S���Y���̃N���X�͎��ۂ̍��W��ێ����Ă��Ȃ��̂ō��W�ɕϊ�
                Vector3 current = Dungeon.IndexToPosition(c.Index, _cellSize);

                if (!first)
                {
                    Vector3 dir = (current - prev).normalized;
                    float dist = (current - prev).magnitude;
                    Physics.RaycastNonAlloc(prev + _cellRayOffset, dir, hit, dist);
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
                    Vector3 dir = new Vector3(neighbour.x, 0, neighbour.y);
                    float dist = (cell.Position - _dungeon.Grid[ny, nx].Position).magnitude;
                    if (Physics.Raycast(cell.Position + _cellRayOffset, dir, dist)) continue;

                    // �ׂ̃Z���Ƀ��C�L���X�g���q�b�g���Ȃ������ꍇ�͐ڑ�����Ă���B
                    _dungeon.ConnectCell(cell.Index.x, cell.Index.y, nx, ny);
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

        void OnDrawGizmos()
        {
            if (_dungeon != null) _dungeon.DrawGridOnGizmos();
            if (_walk != null) _walk.DrawGridOnGizmos(_cellSize);
        }
    }
}