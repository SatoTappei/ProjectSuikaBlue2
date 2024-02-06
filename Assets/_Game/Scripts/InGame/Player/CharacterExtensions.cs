using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PSB.Game
{
    public static class CharacterExtensions
    {
        /// <summary>
        /// ���݈ʒu����ړ��ʕ������w�肵�����Ԃňړ�����B
        /// </summary>
        public static async UniTask MoveAsync(this Transform t, Vector3 movement, float duration, CancellationToken skip)
        {
            Vector3 from = t.position;
            Vector3 to = from + movement;

            // 0���Z�h�~
            if (duration > 0)
            {
                for (float f = 0; f < 1; f += Time.deltaTime / duration)
                {
                    // �X�L�b�v�����ꍇ�͂��̏�Ɏ~�܂�
                    if (skip.IsCancellationRequested) break;

                    t.position = Vector3.Lerp(from, to, f);
                    await UniTask.Yield();
                }
            }

            t.position = to;
        }

        /// <summary>
        /// ���݂̌�������w�肵����]�ʂ��w�肵�����Ԃ������ĉ�]����B
        /// </summary>
        public static async UniTask RotateAsync(this Transform t, float euler, float duration, CancellationToken skip)
        {
            Vector3 from = t.eulerAngles;
            Vector3 to = from + new Vector3(0, euler, 0);

            // 0���Z�h�~
            if (duration > 0)
            {
                for (float f = 0; f < 1; f += Time.deltaTime / duration)
                {
                    // �X�L�b�v�����ꍇ�͂��̏�Ɏ~�܂�
                    if (skip.IsCancellationRequested) break;

                    t.eulerAngles = Vector3.Lerp(from, to, f);
                    await UniTask.Yield();
                }
            }

            t.eulerAngles = to;
        }

        /// <summary>
        /// �W�����v(�ݒu���肪��肩���Ȃ̂Ő���ɓ��삵�Ȃ�)
        /// </summary>
        public static async UniTask JumpAsync(this Rigidbody rb, Vector2Int input, CancellationToken token,
            float horizontalPower, float verticalPower, float interval)
        {
            Vector3 force = Vector3.up * verticalPower + Vector3.right * input.x * horizontalPower;
            rb.AddForce(force, ForceMode.Impulse);

            // �W�����v���ɉ������ɗ͂����������邱�ƂŁA�i���Ɉ����������Ă����z���邱�Ƃ��o����B
            for (float f = 0; f < interval; f += Time.fixedDeltaTime)
            {
                Vector3 velo = rb.velocity;
                velo.x = input.x * horizontalPower;
                rb.velocity = velo;
                // �W�����v�������̃t���[���ł̓��C�L���X�g���n�ʂ��痣��Ȃ��̂ŁA����܂ł̃N�[���^�C����݂���B
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: token);
            }

            //await UniTask.WaitUntil(IsGrounding, cancellationToken: token);
        }
    }
}

// �D��:�X�L�b�v�̃g�[�N�������n���Ă���̂ŉ�]���ɃG�f�B�^�[�~�߂�ƃA�N�Z�X�ł��Ȃ��G���[���o��B