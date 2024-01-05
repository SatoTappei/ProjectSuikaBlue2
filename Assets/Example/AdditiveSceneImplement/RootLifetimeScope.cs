using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Score>(Lifetime.Singleton);
        }
    }
}
