using VContainer;
using VContainer.Unity;

namespace PSB.Architect
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameContext>(Lifetime.Singleton);
        }
    }
}
