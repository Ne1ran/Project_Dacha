using Core.Conditions.Service;
using Core.EntryPoints;
using Core.GameWorld.Service;
using Game.Drop.Service;
using Game.Equipment.Repo;
using Game.Equipment.Service;
using Game.Fertilizers.Service;
using Game.GameMap.Map.Service;
using Game.GameMap.Soil.Repository;
using Game.GameMap.Soil.Service;
using Game.GameMap.Tiles.Repo;
using Game.GameMap.Tiles.Service;
using Game.Interactable.Handlers;
using Game.Interactable.Service;
using Game.Inventory.Repo;
using Game.Inventory.Service;
using Game.Items.Service;
using Game.PieMenu.Service;
using Game.Player.Service;
using Game.PlayMode.Service;
using Game.TimeMove.Repo;
using Game.TimeMove.Service;
using Game.Tools.Service;
using Unity.VisualScripting;
using VContainer;
using VContainer.Unity;

namespace Core.Scopes
{
    public class DachaScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.CurrentScope = this;
            
            builder.Register<InventoryRepo>(Lifetime.Singleton);
            builder.Register<InventoryService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<InventoryMediator>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PlayerService>(Lifetime.Singleton);
            builder.Register<PlayModeService>(Lifetime.Singleton);
            builder.Register<ToolsService>(Lifetime.Singleton);
            builder.Register<DropService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<EquipmentRepo>(Lifetime.Singleton);
            builder.Register<WorldEquipmentManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<EquipmentService>(Lifetime.Singleton);
            builder.Register<TileRepo>(Lifetime.Singleton);
            builder.Register<TileService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<SoilRepo>(Lifetime.Singleton);
            builder.Register<SoilService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<TimeRepo>(Lifetime.Singleton);
            builder.Register<PickUpItemService>(Lifetime.Singleton);
            builder.Register<WorldTileService>(Lifetime.Singleton);
            builder.Register<GameWorldService>(Lifetime.Singleton);
            builder.Register<MapService>(Lifetime.Singleton);
            builder.Register<PieMenuService>(Lifetime.Singleton);
            builder.Register<FertilizerService>(Lifetime.Singleton);
            builder.Register<PieMenuInteractionService>(Lifetime.Singleton);
            builder.Register<ConditionFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();;
            builder.Register<ConditionService>(Lifetime.Singleton);
            builder.Register<ToolUseHandlerFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PieMenuPrepareFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<InteractionHandlerFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<TimeService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<EndDayService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            
            DachaEntryPoint entryPoint = this.AddComponent<DachaEntryPoint>();
            builder.RegisterComponent(entryPoint);
        }
    }
}