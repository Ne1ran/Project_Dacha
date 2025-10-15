using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Resources.Service;
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
using UnityEngine.AddressableAssets;
using VContainer;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("PrepareSoilTool")]
    public class UseSoilToolMenuPrepareHandler : IPieMenuPrepareHandler
    {
        [Inject]
        private readonly InventoryService _inventoryService = null!;
        [Inject]
        private readonly ItemsDescriptor _itemsDescriptor = null!;
        [Inject]
        private readonly ToolsDescriptor _toolsDescriptor = null!;
        [Inject]
        private readonly IResourceService _resourceService = null!;

        public async UniTask<PieMenuItemModel> PrepareAsync(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            List<PieMenuItemSelectionModel> selectionModels = await UpdateSelectionModelsAsync(pieMenuSettings.BaseIcon, token);
            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionHandlerName, pieMenuSettings.Title, pieMenuSettings.Description,
                                             selectionModels);
            return itemModel;
        }

        private async UniTask<List<PieMenuItemSelectionModel>> UpdateSelectionModelsAsync(AssetReference? baseIcon, CancellationToken token)
        {
            List<UniTask<PieMenuItemSelectionModel>> result = new();

            List<InventoryItem> tools = _inventoryService.GetItemsByType(ItemType.TOOL);

            foreach (InventoryItem tool in tools) {
                string toolId = tool.Id;
                ItemDescriptorModel? itemDescriptorModel = _itemsDescriptor.Get(toolId);
                if (itemDescriptorModel == null) {
                    continue;
                }

                ToolsDescriptorModel? toolsDescriptorModel = _toolsDescriptor.Get(toolId);
                if (toolsDescriptorModel == null) {
                    continue;
                }

                if (toolsDescriptorModel.ToolType == ToolType.SOIL) {
                    result.Add(CreateItemSelectionModel(itemDescriptorModel.Icon, toolId, itemDescriptorModel.Name, token));
                }
            }

            if (result.Count == 0) {
                result.Add(CreateItemSelectionModel(baseIcon, string.Empty, "any tool", token));
            }

            PieMenuItemSelectionModel[] items = await UniTask.WhenAll(result);
            return items.ToList();
        }

        private async UniTask<PieMenuItemSelectionModel> CreateItemSelectionModel(AssetReference? icon,
                                                                                  string id,
                                                                                  string name,
                                                                                  CancellationToken token = default)
        {
            Sprite? itemIcon = icon != null ? await _resourceService.LoadAssetAsync<Sprite>(icon.AssetGUID, token) : null;
            return new(id, itemIcon, name);
        }
    }
}