using Core.Descriptors.Service;
using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Scopes;
using Cysharp.Threading.Tasks;
using Game.Calendar.Model;
using Game.Calendar.Service;
using Game.Common.Controller;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Items.Service;
using Game.Plants.Component;
using Game.Plants.Service;
using Game.Player.Controller;
using Game.Player.Service;
using Game.Seeds.Component;
using Game.Seeds.Descriptors;
using Game.Temperature.Service;
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
            IDescriptorService descriptorService = Container.Resolve<IDescriptorService>();

            Vector3 spawnPosition = playerService.Player.transform.position + new Vector3(x, y, z);
            ToolsDescriptor toolsDescriptor = descriptorService.Require<ToolsDescriptor>();
            foreach (ToolsDescriptorModel descriptor in toolsDescriptor.Items) {
                worldItemService.CreateItemInWorldAsync<ToolController>(descriptor.Id, spawnPosition).Forget();
            }
        }

        [ConsoleMethod("createAllSeeds", "Create all seeds by coords relative to the player position")]
        public static void CreateAllSeeds(float x, float y, float z)
        {
            WorldItemService worldItemService = Container.Resolve<WorldItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            IDescriptorService descriptorService = Container.Resolve<IDescriptorService>();

            Vector3 spawnPosition = playerService.Player.transform.position + new Vector3(x, y, z);
            SeedsDescriptor seedsDescriptor = descriptorService.Require<SeedsDescriptor>();
            foreach (SeedsDescriptorModel descriptor in seedsDescriptor.Items) {
                worldItemService.CreateItemInWorldAsync<SeedBagController>(descriptor.Id, spawnPosition).Forget();
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
            IDescriptorService descriptorService = Container.Resolve<IDescriptorService>();
            ItemsDescriptor itemsDescriptor = descriptorService.Require<ItemsDescriptor>();
            Vector3 position = playerService.Player.transform.position + new Vector3(x, y, z);
            foreach (ItemDescriptorModel itemDescriptorModel in itemsDescriptor.Items) {
                worldItemService.CreateItemInWorldAsync(itemDescriptorModel.Id, position).Forget();
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

        [ConsoleMethod("inspectPlant", "Inspect plant on the tile")]
        public static void InspectPlant(int tileId)
        {
            Container.Resolve<PlantsService>().InspectPlant(tileId.ToString());
        }

        [ConsoleMethod("inspectPlantOnSight", "Simulate years of calendar for some checks")]
        public static void InspectPlant()
        {
            PlayerService playerService = Container.Resolve<PlayerService>();
            PlayerController player = playerService.Player;
            IInteractableComponent? playerCurrentLook = player.CurrentLook;
            if (playerCurrentLook is PlantController plantController) {
                Container.Resolve<PlantsService>().InspectPlant(plantController.TileId);
            } 
        }

        private static IObjectResolver Container => AppContext.CurrentScope.Container;
    }
}