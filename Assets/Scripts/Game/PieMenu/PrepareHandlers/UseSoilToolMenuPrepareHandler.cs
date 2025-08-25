using System.Collections.Generic;
using System.Threading;
using Core.Conditions.Service;
using Core.Descriptors.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Interactable.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.PieMenu.Model;
using Game.Tools.Descriptors;
using Game.Tools.Model;
using UnityEngine;
using VContainer;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("PrepareSoilTool")]
    public class UseSoilToolMenuPrepareHandler : IPieMenuPrepareHandler
    {
        [Inject]
        private readonly InventoryService _inventoryService = null!;
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
        [Inject]
        private readonly ConditionService _conditionService = null!;

        public UniTask<PieMenuItemModel> Prepare(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            List<PieMenuItemSelectionModel> selectionModels = UpdateSelectionModels();
            if (selectionModels.Count == 0) {
                Sprite? sprite = Resources.Load<Sprite>(pieMenuSettings.IconPath); // todo neiran remove when go to addressables!!!
                selectionModels.Add(new(string.Empty, sprite, "any tool"));
            }

            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionHandlerName, pieMenuSettings.Title, pieMenuSettings.Description, selectionModels);
            return UniTask.FromResult(itemModel);
        }

        private List<PieMenuItemSelectionModel> UpdateSelectionModels()
        {
            List<PieMenuItemSelectionModel> result = new();

            List<InventoryItem> tools = _inventoryService.GetItemsByType(ItemType.TOOL);
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();

            foreach (InventoryItem tool in tools) {
                ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.ItemDescriptors.Find(toolItem => toolItem.ItemId == tool.Id);
                if (itemDescriptorModel == null) {
                    continue;
                }

                ToolsDescriptorModel? toolsDescriptorModel =
                        toolsDescriptor.ToolsDescriptors.Find(toolItem => toolItem.ToolId == itemDescriptorModel.ItemId);
                if (toolsDescriptorModel == null) {
                    continue;
                }

                if (toolsDescriptorModel.ToolType == ToolType.SOIL) {
                    result.Add(new(tool.Id, itemDescriptorModel.Icon, toolsDescriptorModel.ToolName));
                }
            }

            return result;
        }
    }
}