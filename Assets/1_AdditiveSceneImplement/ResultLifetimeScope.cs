using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class ResultLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 親としてRootLifeTimeScopeが設定されているので
            // このLifetimeScopeに登録したオブジェクトに対しても注入される。
        }
    }
}
