using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace PSB.Game
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [Header("�g�p����e��ݒ�")]
        [SerializeField] PlayerParameterSettings _playerSettings;
        [SerializeField] DungeonParameterSettings _dungeonSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerSettings);
            builder.RegisterInstance(_dungeonSettings);
        }
    }
}
