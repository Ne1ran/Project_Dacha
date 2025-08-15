using Core.Descriptors.Service;
using Core.Scopes;
using Cysharp.Threading.Tasks;
using Game.Inventory.Model;
using Game.Items.Descriptors;
using Game.Items.Service;
using Game.Player.Service;
using Game.TimeMove.Service;
using Game.Tools.Descriptors;
using Game.Tools.Service;
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
            ToolsService toolsService = Container.Resolve<ToolsService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            toolsService.CreateTool(toolName, playerService.Player.transform.position + new Vector3(x, y, z)).Forget();
        }

        [ConsoleMethod("createAllTools", "Create all tools by coords relative to the player position")]
        public static void CreateAllTools(float x, float y, float z)
        {
            ToolsService toolsService = Container.Resolve<ToolsService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            IDescriptorService descriptorService = Container.Resolve<IDescriptorService>();

            Vector3 spawnPosition = playerService.Player.transform.position + new Vector3(x, y, z);
            ToolsDescriptor toolsDescriptor = descriptorService.Require<ToolsDescriptor>();
            foreach (ToolsDescriptorModel descriptor in toolsDescriptor.ToolsDescriptors) {
                toolsService.CreateTool(descriptor.ToolId, spawnPosition).Forget();
            }
        }
        
        [ConsoleMethod("createItem", "Create tool by name and coords relative to the player position")]
        public static void CreateItem(string itemName, ItemType itemType, float x, float y, float z)
        {
            PickUpItemService pickUpItemService = Container.Resolve<PickUpItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            pickUpItemService.DropItemAsync(itemName, itemType, playerService.Player.transform.position + new Vector3(x, y, z)).Forget();
        }
        
        [ConsoleMethod("createAllItems", "Create all items")]
        public static void CreateItems(float x, float y, float z)
        {
            PickUpItemService pickUpItemService = Container.Resolve<PickUpItemService>();
            PlayerService playerService = Container.Resolve<PlayerService>();
            IDescriptorService descriptorService = Container.Resolve<IDescriptorService>();
            ItemsDescriptor itemsDescriptor = descriptorService.Require<ItemsDescriptor>();
            Vector3 position = playerService.Player.transform.position + new Vector3(x, y, z);
            foreach (ItemDescriptorModel itemDescriptorModel in itemsDescriptor.ItemDescriptors) {
                pickUpItemService.DropItemAsync(itemDescriptorModel.ItemId, itemDescriptorModel.ItemType, position).Forget();
            }
        }

        [ConsoleMethod("timePass", "Passes time for N minutes")]
        public static void PassTime(int minutes)
        {
            TimeService timeService = Container.Resolve<TimeService>();
            timeService.PassTime(minutes);
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

        [ConsoleMethod("timePass", "Current time in minutes")]
        public static void CurrentTime()
        {
            TimeService timeService = Container.Resolve<TimeService>();
            Debug.Log($"{timeService.GetTime()}");
        }
        
        private static IObjectResolver Container => AppContext.CurrentScope.Container;
    }
}