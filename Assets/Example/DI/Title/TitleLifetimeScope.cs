using VContainer;
using VContainer.Unity;

namespace PSB.DI.Title
{
    public class TitleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // �C���X�^���X�̐���
            builder.Register<StateOfTitle>(Lifetime.Singleton);
        }
    }
}
