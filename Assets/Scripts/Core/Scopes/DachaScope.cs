using Core.Conditions.Service;
using Core.EntryPoints;
using Core.GameWorld.Service;
using Game.Calendar.Repo;
using Game.Calendar.Service;
using Game.Diseases.Service;
using Game.Drop.Service;
using Game.Equipment.Repo;
using Game.Equipment.Service;
using Game.Evaporation.Service;
using Game.Fertilizers.Service;
using Game.GameMap.Map.Service;
using Game.GameMap.Tiles.Repo;
using Game.GameMap.Tiles.Service;
using Game.Harvest.Service;
using Game.Humidity.Service;
using Game.Interactable.Handlers;
using Game.Interactable.Service;
using Game.Inventory.Repo;
using Game.Inventory.Service;
using Game.Items.Service;
using Game.PieMenu.Service;
using Game.Plants.PlantParams;
using Game.Plants.Repo;
using Game.Plants.Service;
using Game.Player.Service;
using Game.PlayMode.Service;
using Game.Seeds.Service;
using Game.Soil.Repository;
using Game.Soil.Service;
using Game.Sunlight.Service;
using Game.Temperature.Service;
using Game.Testing.Service;
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
            builder.Register<WorldItemService>(Lifetime.Singleton);
            builder.Register<PlayModeService>(Lifetime.Singleton);
            builder.Register<ToolsService>(Lifetime.Singleton);
            builder.Register<DropService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<EquipmentRepo>(Lifetime.Singleton);
            builder.Register<WorldEquipmentManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<EquipmentService>(Lifetime.Singleton);
            builder.Register<TileRepo>(Lifetime.Singleton);
            builder.Register<TileService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<SoilRepo>(Lifetime.Singleton);
            builder.Register<WorldSoilService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();;
            builder.Register<SoilService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<SoilWaterService>(Lifetime.Singleton);
            builder.Register<TimeRepo>(Lifetime.Singleton);
            builder.Register<CalendarService>(Lifetime.Singleton);
            builder.Register<CalendarRepo>(Lifetime.Singleton);
            builder.Register<PickUpItemService>(Lifetime.Singleton);
            builder.Register<WorldTileService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameWorldService>(Lifetime.Singleton);
            builder.Register<MapService>(Lifetime.Singleton);
            builder.Register<PieMenuService>(Lifetime.Singleton);
            builder.Register<FertilizerService>(Lifetime.Singleton);
            builder.Register<PieMenuInteractionService>(Lifetime.Singleton);
            builder.Register<ConditionFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ConditionService>(Lifetime.Singleton);
            builder.Register<SeedsService>(Lifetime.Singleton);
            builder.Register<PlantsService>(Lifetime.Singleton);
            builder.Register<SunlightService>(Lifetime.Singleton);
            builder.Register<AirHumidityService>(Lifetime.Singleton);
            builder.Register<TemperatureService>(Lifetime.Singleton);
            builder.Register<PlantHarvestService>(Lifetime.Singleton);
            builder.Register<WorldPlantsService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PlantDiseaseService>(Lifetime.Singleton);
            builder.Register<PlantsRepo>(Lifetime.Singleton);
            builder.Register<SowSeedHandlerFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ToolUseHandlerFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PieMenuPrepareFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<InteractionHandlerFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PlantsParametersHandlerFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<TimeService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<EndDayService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<CalendarGenerationService>(Lifetime.Singleton);
            builder.Register<TestGameService>(Lifetime.Singleton);
            
            DachaEntryPoint entryPoint = this.AddComponent<DachaEntryPoint>();
            builder.RegisterComponent(entryPoint);
        }
    }
}