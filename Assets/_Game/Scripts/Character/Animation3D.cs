using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation3D : MonoBehaviour
{
    public enum State
    {
        Idle,
        Angry,
        Fun,
        Order,
    }

    [SerializeField] Animator _animator;
    [Header("�A�j���[�V������")]
    [SerializeField] string _idleAnim = "Idle";
    [SerializeField] string _angryAnim = "Angry";
    [SerializeField] string _funAnim = "Fun";
    [SerializeField] string _orderAnim = "Order";

    /// <summary>
    /// �w�肵���A�j���[�V�������Đ�
    /// </summary>
    public void Play(State state)
    {
        if (state == State.Idle) _animator.Play(_idleAnim);
        else if (state == State.Angry) _animator.Play(_angryAnim);
        else if (state == State.Fun) _animator.Play(_funAnim);
        else if (state == State.Order) _animator.Play(_orderAnim);
    }
}
