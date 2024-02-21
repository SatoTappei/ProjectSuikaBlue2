using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using VContainer;

namespace PSB.Game
{
    public class Turret : MonoBehaviour, ICharacter
    {
        [SerializeField] MeshDrawer _meshDrawer;
        [Header("���m�͈͂̃R���C�_�[")]
        [SerializeField] MeshCollider _detectArea;
        [Header("�e�̔��˂̐ݒ�")]
        [SerializeField] Transform _muzzle;
        [SerializeField] GameObject _particle;
        [SerializeField] int _bulletCapacity = 10;
        [SerializeField] float _fireRate = 1.0f;
        [Header("��]���ƂȂ�I�u�W�F�N�g")]
        [SerializeField] Transform _rotateAxis;
        [Header("�e���v�Z�̐ݒ�")]
        [Range(0, 1.0f)]
        [SerializeField] float _quadMiddleProgress = 0.5f;
        [SerializeField] float _quadMiddleHeight = 0;
        [SerializeField] float _quadRightHeight = 0;
        [SerializeField] float _tranjectoryVertexDelta = 0.2f;
        [SerializeField] float _fireRange = 5.0f;
        [Header("�v���C���[�Ɍ������x")]
        [SerializeField] float _lookSpeed = 1.0f;
        [Header("�������ݒ�")]
        [SerializeField] Vector2Int _spawnIndex;
        [SerializeField] bool _randomSpawn;
        [Header("�n�ʂ���̍���")]
        [SerializeField] float _groundOffset;
        [Header("�M�Y���ɕ`��")]
        [SerializeField] bool _drawOnGizmos = true;

        GameState _gameState;
        DungeonManager _dungeonManager;
        ObjectPool _particlePool;
        Vector3 _defaultLookAt;
        Vector3 _lookAt;
        Vector2Int _currentIndex;
        // (0,0)�����_��zy���ʏ�̓񎟊֐��̃O���t
        IReadOnlyList<Vector3> _quad;
        // �v���C���[�����m���t���O
        bool _isDetecting;

        [Inject]
        void Construct(GameState gameState, DungeonManager dungeonManager)
        {
            _gameState = gameState;
            _dungeonManager = dungeonManager;
        }

        void Awake()
        {
            // �����_�������t���O�������Ă���ꍇ�͏������O�ɐ����ʒu��ύX���Ă����B
            if (_randomSpawn && _dungeonManager != null)
            {
                _spawnIndex = Utility.RandomIndex(_dungeonManager.Size);
            }
        }

        void Start()
        {
            Init();
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            RotateAsync(token).Forget();
            FireAsync(token).Forget();
        }

        void Init()
        {
            // �񎟊֐�����e���̃��b�V�����쐬
            _quad = Quad(_fireRange);
            _meshDrawer.Line(_quad, Vector3.left);

            // ���m�͈͂Ƀv���C���[�������������Ă���ꍇ�͂��̈ʒu�������悤�ɐݒ�
            _detectArea.OnTriggerStayAsObservable()
                .Where(c => c.CompareTag(Const.PlayerTag))
                .Subscribe(c => { _lookAt = c.transform.position; _isDetecting = true; });
            // �����������Ă��Ȃ��ꍇ�̓f�t�H���g�̑O�����������悤�ɐݒ�
            _detectArea.OnTriggerExitAsObservable()
                .Where(c => c.CompareTag(Const.PlayerTag))
                .Subscribe(c => { _lookAt = _defaultLookAt; _isDetecting = false; });

            // �p�[�e�B�N���p�̃I�u�W�F�N�g�v�[��
            _particlePool = new(_particle, _bulletCapacity);

            if (_dungeonManager == null) return;

            // ���m�͈͊O�Ƀv���C���[������ꍇ�Ɍ����}�Y���̑O����
            // �v���C���[�̏����ʒu������
            _lookAt = _defaultLookAt = _dungeonManager.GetStartCell().Position;

            // �����ʒu
            Vector3 v = _dungeonManager.GetCell(_spawnIndex).Position;
            v.y += _groundOffset;
            SetPosition(v, _spawnIndex);
        }

        // �^���b�g����]������B
        async UniTaskVoid RotateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _rotateAxis.forward = Forward();

                await UniTask.Yield(token);
            }
        }

        // ���m���t���O�������Ă���ꍇ�A���Ԋu�Ŏˌ��B
        async UniTaskVoid FireAsync(CancellationToken token)
        {
            if (_gameState != null)
            {
                // ���������̃t���O�����܂Ŏˌ����Ȃ��B
                await UniTask.WaitUntil(() => _gameState.IsInGameReady, cancellationToken: token);
            }

            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() => _isDetecting, cancellationToken: token);
                if (RaycastToPlayer()) Fire();
                await UniTask.WaitForSeconds(_fireRate, cancellationToken: token);
            }
        }

        // �}�Y���̑O�����Ƀv���C���[�����m���郌�C�L���X�g
        bool RaycastToPlayer()
        {
            return Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, _fireRange) &&
                hit.collider.CompareTag(Const.PlayerTag);
        }

        // zy���ʏ�ɓ񎟊֐��̃O���t��V�����z�������ĕԂ��B
        IReadOnlyList<Vector3> Quad(float distance)
        {
            // ���_�A���_��������̋�������x�������Ɉړ������_�A���̊Ԃ̓_
            Vector2 p = Vector2.zero;
            Vector2 r = new Vector2(distance, 0);
            Vector2 q = Vector2.Lerp(p, r, _quadMiddleProgress);
            // ���_�ȊO��2�_�͔C�ӂ̍����ɕύX
            q.y = _quadMiddleHeight;
            r.y = _quadRightHeight;

            // 3���_��ʂ�悤�ȓ񎟊֐����v�Z
            Quadratic quad = new(p, q, r);

            // �}�Y���̈ʒu�����_�Ƃ���z�������ɐL�т�Ȑ�
            List<Vector3> list = new();
            for (float i = 0; i < distance; i += _tranjectoryVertexDelta)
            {
                i = Mathf.Min(i, distance);

                Vector3 v = Vector3.forward * i + Vector3.up * quad.GetY(i);
                list.Add(v);
            }
            
            return list;
        }

        // �}�Y���������������v�Z
        Vector3 Forward()
        {
            Vector3 a = _lookAt;
            a.y = 0;
            Vector3 b = _rotateAxis.position;
            b.y = 0;
            Vector3 dir = MyMath.Normalize(a - b);

            return Vector3.Lerp(_rotateAxis.forward, dir, Time.deltaTime * _lookSpeed);
        }

        // ���W��ύX���邱�Ƃňړ�����B
        // ���[���h���W�Ƃ͕ʂɃO���b�h��̈ʒu���Ǘ����Ă���̂ŕK���Z�b�g�ň����B
        void SetPosition(Vector3 position, Vector2Int index)
        {
            if (_dungeonManager == null) return;

            // �Z���ɓo�^���Ă���L�����N�^�[�����X�V
            _dungeonManager.SetCharacter(CharacterKey.Dummy, _currentIndex, null);

            transform.position = position;
            _currentIndex = index;

            _dungeonManager.SetCharacter(CharacterKey.Turret, _currentIndex, this);
        }

        // �ˌ�
        void Fire()
        {
            AudioPlayer.Play(AudioKey.TurretFireSE, AudioPlayer.PlayMode.SE);

            // �p�[�e�B�N���͍Đ��I����ɔ�A�N�e�B�u�ɂȂ�̂ŁA�߂��������K�v�Ȃ��B
            GameObject particle = _particlePool.Rent();
            if (particle != null)
            {
                foreach (ParticleSystem p in particle.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Play();
                }
                particle.transform.position = _muzzle.transform.position;
            }

            // ���G�ɂȂ�̂Œe���ʂ�ł͂Ȃ������̃��C�L���X�g�Ŕ���
            if(Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, _fireRange))
            {
                // �v���C���[�𔻒�
                if(hit.collider.CompareTag(Const.PlayerTag) &&
                   hit.collider.TryGetComponent(out ICharacter player))
                {
                    player.Damage();
                }
            }
        }

        // �_���[�W
        void IDamageReceiver.Damage()
        {
        }

        void OnDrawGizmos()
        {
            if(_drawOnGizmos) DrawQuadOnGizmos();
        }

        // �񎟊֐��̃O���t���M�Y���ɕ`��
        void DrawQuadOnGizmos()
        {
            if (_quad == null) return;

            Gizmos.color = Color.green;
            foreach(Vector3 v in  _quad)
            {
                Gizmos.DrawSphere(v, 0.2f); // �傫���͓K��
            }
        }
    }
}
