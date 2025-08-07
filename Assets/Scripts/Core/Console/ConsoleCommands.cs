using Core.Descriptors.Service;
using Core.Scopes;
using Cysharp.Threading.Tasks;
using Game.Player.Service;
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
        
        private static IObjectResolver Container => AppContext.CurrentScope.Container;
    }
}