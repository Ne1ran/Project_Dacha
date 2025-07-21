using Cysharp.Threading.Tasks;
using Game.Inventory.Service;
using JetBrains.Annotations;

namespace Game.Tools.Service
{
    [UsedImplicitly]
    public class ToolsService
    {
        private readonly InventoryService _inventoryService;

        public ToolsService(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public UniTask CreateTool()
        {
            return UniTask.CompletedTask;
        }
    }
}