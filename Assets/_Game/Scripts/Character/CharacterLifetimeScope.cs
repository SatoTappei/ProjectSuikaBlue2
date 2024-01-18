using VContainer;
using VContainer.Unity;

namespace PSB.Game
{
    public class CharacterLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<TalkState>(Lifetime.Singleton);
        }
    }
}
