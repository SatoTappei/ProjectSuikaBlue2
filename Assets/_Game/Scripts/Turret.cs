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
        [Header("�f�o�b�O�p: �M�Y���ɕ`��")]
        [SerializeField] bool _debugDraw = true;

        [SerializeField] float _lerp = 0.5f;
        [SerializeField] float _distance = 5.0f;
        [SerializeField] float _centerHeight = 0;
        [SerializeField] float _targetHeight = 0;
        [SerializeField] float _space = 0.2f;
        [SerializeField] int _count = 100;

        Quadratic _quadratic;
        Vector3[] _points;

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            _points = new Vector3[_count];

            // ���m�͈͂̃��b�V�����쐬
            _solidArcMesh.Create(-_detectAngle, _detectAngle, _detectRadius);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                LineOfFire(_distance);
                await UniTask.WaitForSeconds(0.1f, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        // �ː���`��
        void LineOfFire(float distance)
        {
            Vector2 p = Vector2.zero;
            Vector2 r = new Vector2(distance, 0);
            Vector2 q = Vector2.Lerp(p, r, _lerp);
            q.y = _centerHeight;
            r.y = _targetHeight;

            _quadratic ??= new(p, q, r);
            _quadratic.Function(p, q, r);

            for (int i = 0; i < _count; i++)
            {
                Vector3 s = _muzzle.position;
                s.z += i * _space;
                s.y += _quadratic.GetY(i);

                _points[i] = s;
            }
        }

        void OnDrawGizmos()
        {
            if (_debugDraw) _solidArcMesh.DebugDraw();

            if (!Application.isPlaying) return;

            for (int i = 0; i < _points.Length; i++)
            {
                if (i == 0 || i == _points.Length / 2 - 1 || i == _points.Length - 1)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }

                Gizmos.DrawSphere(_points[i], 0.2f);
            }
        }
    }
}
