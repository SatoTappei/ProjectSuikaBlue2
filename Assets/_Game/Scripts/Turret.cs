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
        [Header("扇形の検知する範囲")]
        [SerializeField] SolidArcMesh _solidArcMesh;
        [SerializeField] float _detectAngle = 45;
        [SerializeField] float _detectRadius = 3.0f;
        [Header("弾を発射するマズル")]
        [SerializeField] Transform _muzzle;
        [Header("弾道の設定")]
        [SerializeField] float _lerp = 0.5f;
        [SerializeField] float _centerHeight = 0;
        [SerializeField] float _targetHeight = 0;
        [SerializeField] float _space = 0.2f;
        [SerializeField] int _length = 100;
        [Header("デバッグ用: ギズモに描画")]
        [SerializeField] bool _drawGizmos = true;

        // デバッグ用、本来は外部から距離を渡す
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
            // 弾道を構成する頂点
            _points = new Vector3[_length];

            // 検知範囲のメッシュを作成
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

        // 弾道を計算
        void Trajectory(float distance)
        {
            // 原点、原点から引数の距離だけx軸方向に移動した点、その間の点
            Vector2 p = Vector2.zero;
            Vector2 r = new Vector2(distance, 0);
            Vector2 q = Vector2.Lerp(p, r, _lerp);
            // 原点以外の2点は任意の高さに変更
            q.y = _centerHeight;
            r.y = _targetHeight;
            
            // 3頂点を通るような二次関数を計算
            _quadratic ??= new(p, q, r);
            _quadratic.Function(p, q, r);

            // マズルの位置を原点としたz軸方向に伸びる曲線
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

        // 弾道をギズモに描画
        void DrawTrajectoryOnGizmos()
        {
            if (_points == null) return;

            Gizmos.color = Color.green;
            foreach(Vector3 v in  _points)
            {
                Gizmos.DrawSphere(v, 0.2f); // 大きさは適当
            }
        }
    }
}
