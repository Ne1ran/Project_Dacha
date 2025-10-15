using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Scopes;
using Cysharp.Threading.Tasks;
using Game.Calendar.Model;
using Game.Calendar.Service;
using Game.Common.Controller;
using Game.Evaporation.Service;
using Game.GameMap.Tiles.Component;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Items.Service;
using Game.Plants.Component;
using Game.Plants.Model;
using Game.Plants.Service;
using Game.Player.Controller;
using Game.Player.Service;
using Game.Seeds.Component;
using Game.Seeds.Descriptors;
using Game.Temperature.Service;
using Game.Testing.Service;
using Game.Tools.Component;
using Game.Tools.Descriptors;
using IngameDebugConsole;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace Core.Console
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ConsoleCommands
    {
        [ConsoleMethod("debugTest", "Debug Test")]
        public static void DebugTest()
        {
            Debug.LogWarning("Testing right now...");
        }

        [ConsoleMethod("createTool", "Create tool by name and coords relative to the player position")]
        public static void CreateTool(string toolName, float x, float y, float z)
        {
            WorldItemService worldItemService = Container.Resolve<WorldItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            worldItemService.CreateItemInWorldAsync<ToolController>(toolName, playerService.Player.transform.position + new Vector3(x, y, z))
                            .Forget();
        }

        [ConsoleMethod("createAllTools", "Create all tools by coords relative to the player position")]
        public static void CreateAllTools(float x, float y, float z)
        {
            WorldItemService worldItemService = Container.Resolve<WorldItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            Vector3 spawnPosition = playerService.Player.transform.position + new Vector3(x, y, z);
            ToolsDescriptor toolsDescriptor = Container.Resolve<ToolsDescriptor>();
            foreach (string toolId in toolsDescriptor.Items.Keys) {
                worldItemService.CreateItemInWorldAsync<ToolController>(toolId, spawnPosition).Forget();
            }
        }

        [ConsoleMethod("createAllSeeds", "Create all seeds by coords relative to the player position")]
        public static void CreateAllSeeds(float x, float y, float z)
        {
            WorldItemService worldItemService = Container.Resolve<WorldItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            Vector3 spawnPosition = playerService.Player.transform.position + new Vector3(x, y, z);
            SeedsDescriptor seedsDescriptor = Container.Resolve<SeedsDescriptor>();
            foreach (string toolId in seedsDescriptor.Items.Keys) {
                worldItemService.CreateItemInWorldAsync<SeedBagController>(toolId, spawnPosition).Forget();
            }
        }

        [ConsoleMethod("createItem", "Create tool by name and coords relative to the player position")]
        public static void CreateItem(string itemName, float x, float y, float z)
        {
            WorldItemService worldItemService = Container.Resolve<WorldItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            worldItemService.CreateItemInWorldAsync<ItemController>(itemName, playerService.Player.transform.position + new Vector3(x, y, z))
                            .Forget();
        }

        [ConsoleMethod("createAllItems", "Create all items")]
        public static void CreateItems(float x, float y, float z)
        {
            WorldItemService worldItemService = Container.Resolve<WorldItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            ItemsDescriptor itemsDescriptor = Container.Resolve<ItemsDescriptor>();
            Vector3 position = playerService.Player.transform.position + new Vector3(x, y, z);
            foreach (string id in itemsDescriptor.Items.Keys) {
                worldItemService.CreateItemInWorldAsync(id, position).Forget();
            }
        }

        [ConsoleMethod("timePass", "Passes time for N minutes")]
        public static void PassTime(int minutes)
        {
            TimeService timeService = Container.Resolve<TimeService>();
            timeService.PassTime(minutes);
        }

        [ConsoleMethod("currentTemperature", "Passes time for N minutes")]
        public static void CurrentTemperature()
        {
            TemperatureService temperatureService = Container.Resolve<TemperatureService>();
            float currentTemperature = temperatureService.GetCurrentTemperature();
            Debug.LogWarning($"Current temperature is {currentTemperature}!");
        }

        [ConsoleMethod("endDay", "Ends current day")]
        public static void EndDay()
        {
            TimeService timeService = Container.Resolve<TimeService>();
            timeService.EndDay();
        }

        [ConsoleMethod("startDay", "Technically start new day")]
        public static void StartDay()
        {
            TimeService timeService = Container.Resolve<TimeService>();
            timeService.StartDay();
        }

        [ConsoleMethod("getCurrentTime", "Current time in minutes")]
        public static void CurrentTime()
        {
            TimeService timeService = Container.Resolve<TimeService>();
            Debug.Log($"Current global time in minutes={timeService.GetPassedGlobalTime()}");
        }

        [ConsoleMethod("getDayAndMonth", "Current day and month")]
        public static void GetDayAndMonth()
        {
            TimeService timeService = Container.Resolve<TimeService>();
            TimeModel timeModel = timeService.GetToday();
            Debug.Log($"Today is ={(MonthType) timeModel.CurrentMonth} {timeModel.CurrentDay}");
        }

        [ConsoleMethod("showNotification", "Show notification of type")]
        public static void ShowNotification(NotificationType type)
        {
            NotificationManager notificationManager = Container.Resolve<NotificationManager>();
            notificationManager.ShowNotification(type).Forget();
        }

        [ConsoleMethod("simulateTime", "Simulate time in days with some delay")]
        public static async UniTask SimulateTime(int days)
        {
            TimeService timeService = Container.Resolve<TimeService>();
            for (int i = 0; i < days; i++) {
                await UniTask.Delay(100);
                timeService.EndDay();
            }
        }

        [ConsoleMethod("simulateMonthAdv", "Simulate month in calendar for some checks")]
        public static void SimulateMonthAdv(MonthType monthType, int times)
        {
            Container.Resolve<CalendarGenerationService>().Simulate(monthType, times);
        }

        [ConsoleMethod("simulateAllMonths", "Simulate months in calendar for some checks")]
        public static void SimulateAllMonths(int times)
        {
            Container.Resolve<CalendarGenerationService>().SimulateAll(times);
        }

        [ConsoleMethod("simulateYear", "Simulate year of calendar for some checks")]
        public static void SimulateYear()
        {
            Container.Resolve<CalendarGenerationService>().SimulateYear();
        }

        [ConsoleMethod("simulateYears", "Simulate years of calendar for some checks")]
        public static void SimulateYears(int times)
        {
            Container.Resolve<CalendarGenerationService>().SimulateYears(times);
        }

        [ConsoleMethod("testEvaporation", "Test evaporation of water from the soil")]
        public static void TestWaterEvaporation(float millimetersAmount)
        {
            Container.Resolve<SoilWaterService>().CalculateEvaporation(millimetersAmount);
        }

        [ConsoleMethod("setTestEnvironment", "Set test environment")]
        public static void SetTestEnvironment(int month, int day, bool needShovel)
        {
            Container.Resolve<TimeService>().SetTime(month, day);
            TestGameService testGameService = Container.Resolve<TestGameService>();
            testGameService.RemoveAllPlants();
            if (needShovel) {
                testGameService.ShovelAll();
            }

            testGameService.TiltAll();
        }

        [ConsoleMethod("testPlantAll", "Plant test environment with selected plant")]
        public static void TestPlantAll(string plantId)
        {
            TestGameService testGameService = Container.Resolve<TestGameService>();
            testGameService.PlantAll(plantId, 100f, 100f);
        }

        [ConsoleMethod("testOneWeek", "Test one week with test environment")]
        public static void TestOneWeek(float waterAmount)
        {
            TestGameService testGameService = Container.Resolve<TestGameService>();
            testGameService.WaterAll(waterAmount);
            SimulateTime(7).Forget();
        }

        [ConsoleMethod("testFertilizers", "Add fertilizers on test environment")]
        public static void TestFertilizers(string fertilizerId, float fertMass)
        {
            TestGameService testGameService = Container.Resolve<TestGameService>();
            testGameService.UseFertilizer(fertilizerId, fertMass);
        }

        [ConsoleMethod("testEvaporationYear", "Test evaporation for one year")]
        public static void TestWaterEvaporationYear(float millimetersAmount)
        {
            TimeService timeService = Container.Resolve<TimeService>();
            timeService.SetTime(1, 1);
            SoilWaterService soilWaterService = Container.Resolve<SoilWaterService>();
            float currentEvaporation = soilWaterService.CalculateEvaporation(millimetersAmount);
            TimeModel today = timeService.GetToday();
            Debug.LogWarning($"Evaporation. Month={today.CurrentMonth} Day={today.CurrentDay} StartWater={millimetersAmount}, Evaporation={currentEvaporation}");
            for (int i = 0; i < 365; i++) {
                TimeModel newDay = timeService.EndDay();
                float evaporation = soilWaterService.CalculateEvaporation(millimetersAmount);
                Debug.LogWarning($"Evaporation. Month={newDay.CurrentMonth} Day={newDay.CurrentDay} StartWater={millimetersAmount}, Evaporation={evaporation}");
            }
        }

        [ConsoleMethod("inspectPlant", "Inspect plant on the tile")]
        public static void InspectPlant(int tileId)
        {
            PlantInspectionModel? plantInspectionModel = Container.Resolve<PlantsService>().InspectPlant(tileId.ToString());
            if (plantInspectionModel == null) {
                Debug.LogWarning($"Plant inspection model could not be made! TileId={tileId}");
                return;
            }

            Debug.Log($"You have inspected plant. Some parameters: Id={plantInspectionModel.PlantId} Health={plantInspectionModel.Health}, "
                      + $"Immunity={plantInspectionModel.Immunity}, Stage={plantInspectionModel.CurrentStage}, StageGrowth={plantInspectionModel.StageGrowth}");
        }

        [ConsoleMethod("inspectPlantOnSight", "Simulate years of calendar for some checks")]
        public static void InspectPlant()
        {
            PlayerService playerService = Container.Resolve<PlayerService>();
            PlayerController player = playerService.Player;
            IInteractableComponent? playerCurrentLook = player.CurrentLook;
            PlantsService plantsService = Container.Resolve<PlantsService>();
            PlantInspectionModel? plantInspectionModel = playerCurrentLook switch {
                    PlantController plantController => plantsService.InspectPlant(plantController.TileId),
                    TileController tileController => plantsService.InspectPlant(tileController.TileModel.Id),
                    _ => null
            };

            if (plantInspectionModel == null) {
                Debug.LogWarning("Plant inspection model on current look could not be made!");
                return;
            }

            Debug.Log($"You have inspected plant. Some parameters: Id={plantInspectionModel.PlantId} Health={plantInspectionModel.Health}, "
                      + $"Immunity={plantInspectionModel.Immunity}, Stage={plantInspectionModel.CurrentStage}, StageGrowth={plantInspectionModel.StageGrowth}");
        }

        private static IObjectResolver Container => AppContext.CurrentScope.Container;
    }
}