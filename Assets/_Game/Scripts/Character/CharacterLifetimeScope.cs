using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PSB.Game
{
    public class CharacterLifetimeScope : LifetimeScope
    {
        [Header("�L�����N�^�[�̐ݒ�")]
        [SerializeField] CharacterSettings _characterSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_characterSettings);
            builder.Register<Talk>(Lifetime.Singleton);
        }
    }
}
