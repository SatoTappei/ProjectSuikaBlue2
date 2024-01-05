using VContainer;
using VContainer.Unity;

namespace PSB.DI.Title
{
    public class TitleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // インスタンスの生成
            builder.Register<StateOfTitle>(Lifetime.Singleton);
        }
    }
}
