using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraController : MonoBehaviour
{
    [Header("カメラ側の各オブジェクト")]
    [SerializeField] Transform _root;
    [SerializeField] Transform _parent;
    [SerializeField] Transform _camera;
    [Header("追従する対象")]
    [SerializeField] Transform _follow;
    [Header("位置のオフセット")]
    [SerializeField] Vector3 _offset;

    void Start()
    {
        if (_follow == null) return;

        UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTaskVoid UpdateAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // rootオブジェクトは追従対象と同じ位置
            _root.position = _follow.position;
            // parentオブジェクトで位置のオフセット
            _parent.localPosition = _offset;
            // camera本体が回転して対象と同じ方向を向く
            _camera.forward = _follow.forward;

            await UniTask.Yield(PlayerLoopTiming.LastUpdate);
        }
    }
}
