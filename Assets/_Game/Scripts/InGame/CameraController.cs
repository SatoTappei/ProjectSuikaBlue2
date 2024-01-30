using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraController : MonoBehaviour
{
    [Header("�J�������̊e�I�u�W�F�N�g")]
    [SerializeField] Transform _root;
    [SerializeField] Transform _parent;
    [SerializeField] Transform _camera;
    [Header("�Ǐ]����Ώ�")]
    [SerializeField] Transform _follow;
    [Header("�ʒu�̃I�t�Z�b�g")]
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
            // root�I�u�W�F�N�g�͒Ǐ]�ΏۂƓ����ʒu
            _root.position = _follow.position;
            // parent�I�u�W�F�N�g�ňʒu�̃I�t�Z�b�g
            _parent.localPosition = _offset;
            // camera�{�̂���]���đΏۂƓ�������������
            _camera.forward = _follow.forward;

            await UniTask.Yield(PlayerLoopTiming.LastUpdate);
        }
    }
}
