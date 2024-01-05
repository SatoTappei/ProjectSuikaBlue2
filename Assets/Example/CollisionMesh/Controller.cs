using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.CollisionMesh
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] CollisionMesh _mesh;
        [SerializeField] float _startAngle;
        [SerializeField] float _endAngle = 45.0f;
        [SerializeField] float _radius = 3.0f;
        [SerializeField] bool _debugDraw = true;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _mesh.Create(_startAngle, _endAngle, _radius);
                await UniTask.WaitForSeconds(0.1f, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        void OnDrawGizmos()
        {
            if (_debugDraw) _mesh.DebugDraw();
        }
    }
}