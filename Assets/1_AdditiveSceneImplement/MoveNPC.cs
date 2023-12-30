using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class MoveNPC : MonoBehaviour
    {
        [SerializeField] Transform[] _wayPoints;
        [SerializeField] float _speed = 1.0f;

        GameContext _context;

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        [Inject]
        void Construct(GameContext context)
        {
            _context = context;
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            bool active = true;

            if (_context != null)
            {
                _context.IsPauseObservable.Skip(1).Subscribe(b => active = !b).AddTo(gameObject);
                _context.IsPlayerInputObservable.Skip(1).Subscribe(b => active = b).AddTo(gameObject);
            }

            List<Vector3> path = CreatePath();
            Transform transform = this.transform;
            Vector3 from = path[0];
            Vector3 to = path[1];
            int currentIndex = 1; // 0 が初期位置なので次に向かう 1 が初期値
            float elapsed = 0;

            // 経由地点の先頭が初期位置
            transform.position = path[0];

            while (!token.IsCancellationRequested)
            {
                if (active)
                {
                    transform.position = Vector3.MoveTowards(from, to, elapsed);

                    if (transform.position == to)
                    {
                        from = to;
                        currentIndex++;
                        currentIndex %= path.Count;
                        to = path[currentIndex];
                        elapsed = 0;
                    }

                    elapsed += Time.deltaTime * _speed;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        // 経由地点を往復する経路
        List<Vector3> CreatePath()
        {
            List<Vector3> path = new();
            for (int i = 0; i < _wayPoints.Length; i++)
            {
                Vector3 p = _wayPoints[i].position;
                p.y = transform.position.y;
                path.Add(p);
            }
            for (int i = _wayPoints.Length - 2; i > 0; i--)
            {
                Vector3 p = _wayPoints[i].position;
                p.y = transform.position.y;
                path.Add(p);
            }

            return path;
        }
    }
}
