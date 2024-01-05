using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PSB.DI.PlayerControl
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<KeyboardInput>(Lifetime.Scoped);
            builder.Register<IObjectResolver, Container>(Lifetime.Scoped);
        }
    }
}