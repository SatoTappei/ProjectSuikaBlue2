using VContainer;
using VContainer.Unity;

namespace PSB.Game
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameState>(Lifetime.Singleton);
        }
    }
}