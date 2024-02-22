using VContainer;
using VContainer.Unity;
using UnityEngine;
using PSB.Game.BT;

namespace PSB.Game
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [Header("各種機能")]
        [SerializeField] TileBuilder _tileBuilder;
        [SerializeField] EntityCreator _entityCreator;
        [SerializeField] UiManager _uiManager;
        [Header("使用する各種設定")]
        [SerializeField] PlayerParameterSettings _playerSettings;
        [SerializeField] DungeonParameterSettings _dungeonSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_tileBuilder != null) builder.RegisterComponent(_tileBuilder);
            if (_entityCreator != null) builder.RegisterComponent(_entityCreator);
            if (_uiManager != null) builder.RegisterComponent(_uiManager);
            if (_playerSettings != null) builder.RegisterInstance(_playerSettings);
            if (_dungeonSettings != null) builder.RegisterInstance(_dungeonSettings);

            builder.Register<BlackBoard>(Lifetime.Singleton);

            if (_dungeonSettings != null && _tileBuilder != null && _entityCreator != null)
            {
                builder.Register<DungeonManager>(Lifetime.Singleton);
            }
        }
    }
}
