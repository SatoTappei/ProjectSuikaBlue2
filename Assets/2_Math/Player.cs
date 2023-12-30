using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.CrossProduct
{
    public class Player : MonoBehaviour
    {
        [SerializeField] Transform[] _enemies;
        [SerializeField] float _rotSpeed = 5.0f;
        [SerializeField] float _viewAngle = 90.0f;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);

                // 近い順にクイックソート
                MyMath.Sort(_enemies, transform.position);

                if (TryGetTargetPosition(out Vector3 pos))
                {
                    await RotateAsync(pos, _rotSpeed, token);
                }
            }
        }
        
        // 内積で視界内の敵を判定し位置を返す
        bool TryGetTargetPosition(out Vector3 pos)
        {
            foreach (Transform t in _enemies)
            {
                float dot = MyMath.Dot(transform.forward, (t.position - transform.position).normalized);
                float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
                if (deg <= _viewAngle / 2)
                {
                    pos = t.position;
                    return true;
                }
            }

            pos = default;
            return false;
        }

        // 線形補完で向く
        async UniTask RotateAsync(Vector3 targetPos, float speed, CancellationToken token)
        {
            Vector3 to = targetPos - transform.position;
            Vector3 from = transform.forward;

            for (float t = 0; t < 1.0f; t += Time.deltaTime * speed)
            {
                transform.forward = MyMath.Lerp(from, to, t);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            transform.forward = to;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.green * 0.2f;
            Vector3 center = new Vector3(transform.position.x, 0.01f, transform.position.z);
            Vector3 from = Quaternion.Euler(0, -_viewAngle / 2, 0) * transform.forward;
            UnityEditor.Handles.DrawSolidArc(center, Vector3.up, from, _viewAngle, 10);
        }
#endif
    }
}
