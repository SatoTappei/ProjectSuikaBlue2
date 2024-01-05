using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using VContainer;

namespace PSB.DI.Title
{
    /// <summary>
    /// タイトル画面の各コンテンツ。
    /// VContainerによって注入された、タイトルの状態を管理するクラスの値を監視し、
    /// 自身の状態に遷移した場合に開く、他の状態に遷移した場合は閉じる。
    /// </summary>
    public class TitleContent : MonoBehaviour
    {
        [Header("状態に紐づける")]
        [SerializeField] TitleState _contentState;
        [Header("操作するUI")]
        [SerializeField] Button _openButton;
        [SerializeField] Button _backButton;
        [Tooltip("閉じるボタンを押したタイミングでこのオブジェクトが非表示になる")]
        [SerializeField] GameObject _root;

        StateOfTitle _state;

        [Inject]
        public void Construct(StateOfTitle state)
        {
            _state = state;
        }

        void Start()
        {
            // 開くボタンをクリックした際に、現在の状態をこのコンテンツの状態に変更する
            _openButton.OnClickAsObservable().Subscribe(_ => 
            {
                _state.ChangeState(_contentState);
            }).AddTo(gameObject);

            // 現在の状態がこのコンテンツの状態の場合は開く、それ以外の場合は閉じる
            _state.CurrentState.Subscribe(state => 
            {
                _root.SetActive(state == _contentState);
            }).AddTo(gameObject);

            // 戻るボタンをクリックをした際に、現在の状態をトップに変更することで閉じる
            _backButton.OnClickAsObservable().Subscribe(_ => 
            {
                _state.ChangeState(TitleState.Top);
            }).AddTo(gameObject);
        }
    }
}
