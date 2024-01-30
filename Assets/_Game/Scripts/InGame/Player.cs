using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    // ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g���ړ��Ɖ�]�ǂ�����s��
    public class Player : MonoBehaviour
    {
        [Header("�n�ʂ���̍���")]
        [SerializeField] float _groundOffset;

        void Start()
        {
            Init();
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void Init()
        {
            // �n�ʂɗ�������
            transform.Translate(Vector3.up * _groundOffset);
        }

        async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            Direction forward = Direction.North;
            Transform transform = this.transform;
            while (!token.IsCancellationRequested)
            {
                using (CancellationTokenSource skipTokenSource = new())
                {
                    // ���͂̃��b�Z�[�W�����ł���܂őҋ@
                    KeyInputMessage msg = await MessageBroker.Default
                        .Receive<KeyInputMessage>().ToUniTask(useFirstValue: true, token);

                    // �ړ��������͉�]
                    if (msg.IsMoveKey(out KeyCode moveKey))
                    {
                        Vector3 move = moveKey.ToNormalizedDirectionVector(forward);
                        await transform.MoveAsync(move, 0.5f, skipTokenSource.Token);
                    }
                    else if (msg.IsRotateKey(out KeyCode rotKey))
                    {
                        float rot = rotKey.To90DegreeRotateAngle();
                        await transform.RotateAsync(rot, 0.5f, skipTokenSource.Token);
                        forward = rotKey.ToTurnedDirection(forward);
                    }
                }
            }
        }
    }
}
