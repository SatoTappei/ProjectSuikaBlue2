using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using UnityEngine.SceneManagement;

namespace PSB.Architect
{
    public sealed class ScoreLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 親としてRootLifeTimeScopeが設定されているので
            // このLifetimeScopeに登録したオブジェクトに対しても注入される。
        }
    }
}