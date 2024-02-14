using VContainer;
using VContainer.Unity;
using UnityEngine;
using PSB.Architect;
using PSB.Game.BT;

namespace PSB.Game
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [Header("�e��@�\")]
        [SerializeField] TileBuilder _tileBuilder;
        [SerializeField] EntityCreator _entityCreator;
        [SerializeField] UiManager _uiManager;
        [Header("�g�p����e��ݒ�")]
        [SerializeField] PlayerParameterSettings _playerSettings;
        [SerializeField] DungeonParameterSettings _dungeonSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_tileBuilder);
            builder.RegisterComponent(_entityCreator);
            builder.RegisterComponent(_uiManager);
            builder.Register<DungeonManager>(Lifetime.Singleton);
            builder.Register<BlackBoard>(Lifetime.Singleton);
            builder.RegisterInstance(_playerSettings);
            builder.RegisterInstance(_dungeonSettings);
        }
    }
}
