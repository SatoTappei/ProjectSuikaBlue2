using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace PSB.DI.Tutorial
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] HelloScreen _helloScreen;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Service>(Lifetime.Singleton);
            builder.RegisterEntryPoint<Presenter>(Lifetime.Singleton);
            builder.RegisterComponent(_helloScreen);
        }
    }
}
