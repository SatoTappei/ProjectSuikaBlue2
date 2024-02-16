using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using PSB.Game.FSM; // �X�e�[�g�}�V������͐�p�̖��O��Ԃɂ܂Ƃ߂Ă���
using StateKey = PSB.Game.FSM.State.StateKey;
using PSB.Game.BT;

namespace PSB.Game
{
    public class Enemy : MonoBehaviour, ICharacter
    {
        /// <summary>
        /// �A�j���[�V�����Đ��p�̃L�[
        /// </summary>
        public enum AnimationKey { Idle, Kick, Walk, Sprint }
        
        /// <summary>
        /// �p�[�e�B�N���Đ��p�̃L�[
        /// </summary>
        public enum ParticleKey { Dash }

        [Header("���f������]������")]
        [SerializeField] Transform _model;
        [Header("�A�j���[�V�����̐ݒ�")]
        [SerializeField] Animator _animator;
        [SerializeField] string _idleAnimationName = "Idle";
        [SerializeField] string _kickAnimationName = "Kick";
        [SerializeField] string _walkAnimationName = "Walk";
        [SerializeField] string _sprintAnimationName = "Sprint";
        [Header("�e�X�e�[�g")]
        [SerializeField] IdleState _idleState;
        [SerializeField] ChaseState _chaseState;
        [SerializeField] AttackState _attackState;
        [SerializeField] SearchState _searchState;
        [Header("�ǂ̃X�e�[�g����J�n���邩")]
        [SerializeField] StateKey _defaultState = StateKey.Idle;
        [Header("�������ݒ�")]
        [SerializeField] Vector2Int _spawnIndex;
        [SerializeField] Direction _spawnDirection;
        [Header("�n�ʂ���̍���")]
        [SerializeField] float _groundOffset;
        [Header("�����Ƀ��C�L���X�g����ۂɎg����̍���")]
        [SerializeField] float _eyeHeight = 0.5f;
        [Header("����ۂ̃p�[�e�B�N��")]
        [SerializeField] GameObject _particle;
        [SerializeField] Vector3 _particleOffset;
        [SerializeField] int _particleCapacity;

        GameState _gameState;
        DungeonManager _dungeonManager;
        BlackBoard _blackBoard;
        BlackBoard.Private _privateBoard;
        Transform _transform;
        Dictionary<StateKey, State> _states = new(4);
        ObjectPool _particlePool;
        Vector2Int _currentIndex;
        Direction _forward;
        int _idleAnimationHash;
        int _kickAnimationHash;
        int _walkAnimationHash;
        int _sprintAnimationHash;

        /// <summary>
        /// �r�w�C�r�A�c���[�̊e�m�[�h���ǂݏ�������B
        /// </summary>
        public BlackBoard.Private PrivateBoard => _privateBoard ??= _blackBoard.CreatePrivate();

        [Inject]
        void Construct(GameState gameState, DungeonManager dungeonManager, BlackBoard blackBoard)
        {
            _gameState = gameState;
            _dungeonManager = dungeonManager;
            _blackBoard = blackBoard;
        }

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            _transform  = transform;

            // �A�j���[�V�����̃n�b�V��
            _idleAnimationHash = Animator.StringToHash(_idleAnimationName);
            _kickAnimationHash = Animator.StringToHash(_kickAnimationName);
            _walkAnimationHash = Animator.StringToHash(_walkAnimationName);
            _sprintAnimationHash = Animator.StringToHash(_sprintAnimationName);

            // �L�����N�^�[���̍���
            _privateBoard = _blackBoard.CreatePrivate();

            // �X�e�[�g�������ŊǗ�
            _states.Add(StateKey.Idle, _idleState);
            _states.Add(StateKey.Chase, _chaseState);
            _states.Add(StateKey.Attack, _attackState);
            _states.Add(StateKey.Search, _searchState);

            // �e�X�e�[�g�̏���������
            foreach (KeyValuePair<StateKey, State> state in _states)
            {
                state.Value.Awake(gameObject);
            }

            // �p�[�e�B�N���p�̃I�u�W�F�N�g�v�[��
            _particlePool = new(_particle, _particleCapacity);

            // �����ʒu
            Vector3 v = _dungeonManager.GetCell(_spawnIndex).Position;
            v.y += _groundOffset;
            SetPosition(v, _spawnIndex);

            // ��]
            Rotate(_spawnDirection);
            _forward = _spawnDirection;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // �C�ӂ̃X�e�[�g����J�n
            State state = _states[_defaultState];

            while (!token.IsCancellationRequested)
            {
                state = state.Update();
                Debug.Log(state.ToString());

                await UniTask.Yield(token);
            }
        }

        /// <summary>
        /// �J�ڐ�̃X�e�[�g���擾
        /// </summary>
        public bool TryGetState(StateKey key, out State state)
        {
            return _states.TryGetValue(key, out state);
        }

        /// <summary>
        /// �Z���P�ʂ̋����Ńv���C���[�����݂��邩�𔻒�B
        /// �K�v�ɉ����ă��C�L���X�g�Ō����Ă��邩�𔻒�B
        /// </summary>
        public bool DetectPlayer(int cellDistance, bool checkWithinSight)
        {
            // �`�F�r�V�F�t�����ŃZ���P�ʂ̋����𑪂�B
            if (_currentIndex.ChebyshevDistance(_gameState.PlayerIndex) > cellDistance) return false;

            // ���g�̈ʒu�ƃv���C���[�����݂���Z���̊Ԃ����C�L���X�g���A
            // ��Q���������ꍇ�͌����Ă���Ɣ��肷��B
            if (checkWithinSight)
            {
                // ���E���Ղ�Ȃ���Q�����l�����āA��̍������琅���Ƀ��C�L���X�g����B
                Vector3 p = _dungeonManager.GetCell(_gameState.PlayerIndex).Position;
                p.y = _eyeHeight;
                Vector3 q = _transform.position;
                q.y = _eyeHeight;

                bool hit = Physics.Linecast(p, q, Const.DungeonLayer);
#if UNITY_EDITOR
                Debug.DrawLine(p, q, hit ? Color.red : Color.green);
#endif
                return !hit;
            }

            return true;
        }

        /// <summary>
        /// �אڂ��郉���_���ȃZ���ւ̌o�H�T���B
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> PathfindingToNeighbour()
        {
            IReadOnlyList<IReadOnlyCell> adjacent = _dungeonManager.GetCell(_currentIndex).Adjacent;
            Vector2Int neighbour = adjacent[Random.Range(0, adjacent.Count)].Index;

            return Pathfinding(neighbour);
        }

        /// <summary>
        /// ���ݒn����v���C���[�܂ł̌o�H�T���B
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> PathfindingToPlayer()
        {
            return Pathfinding(_gameState.PlayerIndex);
        }

        /// <summary>
        /// ���ݒn����̌o�H�T���B
        /// </summary>
        public IReadOnlyList<IReadOnlyCell> Pathfinding(Vector2Int target)
        {
            return _dungeonManager.Pathfinding(_currentIndex, target);
        }

        /// <summary>
        /// ���W��ύX���邱�Ƃňړ�����B
        /// ���[���h���W�Ƃ͕ʂɃO���b�h��̈ʒu���Ǘ����Ă���̂ŕK���Z�b�g�ň����B
        /// </summary>
        public void SetPosition(Vector3 position, Vector2Int index)
        {
            // �Z���ɓo�^���Ă���L�����N�^�[�����X�V
            _dungeonManager.SetCharacter(CharacterKey.Dummy, _currentIndex, null);
            
            _transform.position = position;
            _currentIndex = index;

            _dungeonManager.SetCharacter(CharacterKey.Enemy, _currentIndex, this);
        }

        /// <summary>
        /// ���[���h���W�ƃO���b�h��̍��W��Ԃ��B
        /// ���[���h���W�Ƃ͕ʂɃO���b�h��̈ʒu���Ǘ����Ă���̂ŕK���Z�b�g�ň����B
        /// </summary>
        public (Vector3 position, Vector2Int index) GetPosition()
        {
            return (_transform.position, _currentIndex);
        }

        /// <summary>
        /// �L�����N�^�[�̃��f����C�ӂ̕����ɐ��`�⊮��p����1�t���[������]������B
        /// �O�����������l�����̕����ɂȂ�B
        /// </summary>
        public void Rotate(Direction direction, float t = 1)
        {
            Quaternion q = direction.ToQuaternion();
            _model.rotation = Quaternion.Lerp(_model.rotation, q, t);
            _forward = direction;
        }

        /// <summary>
        /// �A�j���[�V�������Đ�
        /// </summary>
        public void PlayAnimation(AnimationKey key)
        {
            if (key == AnimationKey.Idle) _animator.Play(_idleAnimationHash);
            else if (key == AnimationKey.Kick) _animator.Play(_kickAnimationHash);
            else if (key == AnimationKey.Walk) _animator.Play(_walkAnimationHash);
            else if (key == AnimationKey.Sprint) _animator.Play(_sprintAnimationHash);
        }

        /// <summary>
        /// �v���C���[�ɑ΂��čU�����s��
        /// </summary>
        public void Attack()
        {
            // �O��������ɕ��������߂�
            Vector2Int target = _currentIndex + _forward.ToIndex();
            IReadOnlyCell cell = _dungeonManager.GetCell(target);

            if (cell.Character != null) cell.Character.Damage();
        }

        /// <summary>
        /// �Z���Ɏ����ȊO�̃L�����N�^�[�����݂��邩�𒲂ׂ�B
        /// </summary>
        public bool IsExistOtherCharacter(Vector2Int index)
        {
            IReadOnlyCell cell = _dungeonManager.GetCell(index);

            // �L�����N�^�[�����Ȃ��ꍇ
            if (cell.Character == null) return false;
            // �����ȊO�̃L�����N�^�[���ǂ���
            return cell.Character != (ICharacter)this;
        }

        /// <summary>
        /// �p�[�e�B�N�����Đ�
        /// </summary>
        public void PlayParticle(ParticleKey _) 
        {
            // ����p�[�e�B�N����1�Ȃ̂ň����̕K�v�Ȃ����ꉞ�B

            // �p�[�e�B�N���͍Đ��I����ɔ�A�N�e�B�u�ɂȂ�̂ŁA�߂��������K�v�Ȃ��B
            GameObject p = _particlePool.Rent();
            if (p != null)
            {
                p.GetComponent<ParticleSystem>().Play();
                p.transform.position = _transform.position + _particleOffset;
            }
        }

        // �_���[�W
        void IDamageReceiver.Damage()
        {
            // �����ŏ�������̂ł͂Ȃ��A�_���[�W�̓��e���L���[�C���O���ĔC�ӂ̃^�C�~���O�ŏ������������ǂ�
            Debug.Log(name + "���_���[�W���󂯂�");
        }
    }
}
