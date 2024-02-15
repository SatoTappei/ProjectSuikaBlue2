using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PSB.Game
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] MeshDrawer _meshDrawer;
        [Header("��`�̌��m����͈�")]
        [SerializeField] SolidArcMesh _solidArcMesh;
        [SerializeField] float _detectAngle = 45;
        [SerializeField] float _detectRadius = 3.0f;
        [Header("�e�𔭎˂���}�Y��")]
        [SerializeField] Transform _muzzle;
        [Header("�e���̐ݒ�")]
        [SerializeField] float _lerp = 0.5f;
        [SerializeField] float _centerHeight = 0;
        [SerializeField] float _targetHeight = 0;
        [SerializeField] float _space = 0.2f;
        [SerializeField] int _length = 100;
        [Header("�f�o�b�O�p: �M�Y���ɕ`��")]
        [SerializeField] bool _drawGizmos = true;

        // �f�o�b�O�p�A�{���͊O�����狗����n��
        [SerializeField] float _distance = 5.0f;

        Quadratic _quadratic;
        Vector3[] _points;

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // �e�����\�����钸�_
            _points = new Vector3[_length];

            // ���m�͈͂̃��b�V�����쐬
            _solidArcMesh.Create(-_detectAngle, _detectAngle, _detectRadius);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                //await UniTask.WaitUntil();
                Trajectory(_distance);
                _meshDrawer.Line(_points);
                await UniTask.WaitForSeconds(0.1f, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        // �e�����v�Z
        void Trajectory(float distance)
        {
            // ���_�A���_��������̋�������x�������Ɉړ������_�A���̊Ԃ̓_
            Vector2 p = Vector2.zero;
            Vector2 r = new Vector2(distance, 0);
            Vector2 q = Vector2.Lerp(p, r, _lerp);
            // ���_�ȊO��2�_�͔C�ӂ̍����ɕύX
            q.y = _centerHeight;
            r.y = _targetHeight;
            
            // 3���_��ʂ�悤�ȓ񎟊֐����v�Z
            _quadratic ??= new(p, q, r);
            _quadratic.Function(p, q, r);

            // �}�Y���̈ʒu�����_�Ƃ���z�������ɐL�т�Ȑ�
            for (int i = 0; i < _length; i++)
            {
                Vector3 s = _muzzle.position;
                s += _muzzle.forward * i * _space;
                s += _muzzle.up * _quadratic.GetY(i);

                _points[i] = s;
            }
        }

        void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                _solidArcMesh.DrawOnGizmos();
                DrawTrajectoryOnGizmos();
            }
        }

        // �e�����M�Y���ɕ`��
        void DrawTrajectoryOnGizmos()
        {
            if (_points == null) return;

            Gizmos.color = Color.green;
            foreach(Vector3 v in  _points)
            {
                Gizmos.DrawSphere(v, 0.2f); // �傫���͓K��
            }
        }
    }
}
