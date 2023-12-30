using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace PSB.DI.Title
{
    public enum TitleState
    {
        Top,
        StageSelect,
        Gacha,
        Option,
    }

    /// <summary>
    /// タイトルの状態を知りたいクラスはこのクラスを参照する。
    /// ものびを継承していないのでインスペクタから割り当てられない。
    /// そこでVContainerでこのクラスのインスタンスを作り、必要なクラスに注入してやる。
    /// 1つのクラスを参照かつ、インスペクターを汚さないかつ、Containerという単位で管理されたものが出来上がる
    /// </summary>
    public class StateOfTitle
    {
        ReactiveProperty<TitleState> _currentState = new(TitleState.Top);

        public IReadOnlyReactiveProperty<TitleState> CurrentState => _currentState;

        /// <summary>
        /// 状態の遷移を行い、遷移にSubscribeされている処理が発火する。
        /// </summary>
        public void ChangeState(TitleState next)
        {
            _currentState.Value = next;
        }
    }
}
