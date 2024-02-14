using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game.FSM
{
    /// <summary>
    /// ステートマシンの各種ステートはこのクラスを継承する。
    /// 各ステートに自身のオブジェクトを渡すことで、ステート側に処理を呼び出してもらう。
    /// </summary>
    [System.Serializable]
    public abstract class State
    {
        public enum StateKey
        {
            Base,
            Idle,
            Chase,
            Attack,
            Search,
        }

        enum Stage
        {
            Enter,
            Stay,
            Exit,
        }

        Stage _stage;
        State _next;
        // Updateを呼ぶ前にAwakeを呼んで初期化済みかのフラグ
        bool _initCompleted;

        /// <summary>
        /// 外部からどのステートなのかを判定するために使用。
        /// </summary>
        public abstract StateKey Key { get; }

        /// <summary>
        /// Updateを呼ぶ前に最初の1回だけ呼び出す。
        /// </summary>
        public void Awake(GameObject self)
        {
            Init(self);
            _initCompleted = true;
        }

        /// <summary>
        /// 1度の呼び出しでステートの段階に応じてEnter,Stay,Exitのうちどれか1つが実行される。
        /// 次の呼び出しで実行されるステートを返す。
        /// </summary>
        public State Update()
        {
            if (!_initCompleted)
            {
                Debug.LogWarning("Awakeを呼んでいないため、初期化していない状態での実行中: " + Key);
            }

            if (_stage == Stage.Enter)
            {
                Enter();
                _stage = Stage.Stay;
            }
            else if (_stage == Stage.Stay)
            {
                Stay();
            }
            else if (_stage == Stage.Exit)
            {
                Exit();
                _stage = Stage.Enter;

                return _next;
            }

            return this;
        }

        protected abstract void Init(GameObject self);
        protected abstract void Enter();
        protected abstract void Stay();
        protected abstract void Exit();

        /// <summary>
        /// 次にプールから取り出した時にEnter以外から始まるのを防ぐ。
        /// プールに戻す際に呼ぶ必要がある。
        /// </summary>
        protected void Reset() => _stage = Stage.Enter;

        /// <summary>
        /// Enter()が呼ばれてかつ、ステートの遷移処理を呼んでいない場合のみ遷移可能。
        /// </summary>
        public bool TryChangeState(State next)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enterが呼ばれる前にステートを遷移することは不可能: {Key} 遷移先: {next}");
                return false;
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"既に別のステートに遷移する処理が呼ばれている: {Key} 遷移先: {next}");
                return false;
            }

            _stage = Stage.Exit;
            _next = next;

            return true;
        }
    }
}
