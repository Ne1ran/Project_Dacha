using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Descriptors.Service;
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
    [Handler("PrepareWaterSoil")]
    public class WaterSoilMenuPrepareHandler : IPieMenuPrepareHandler
    {
        [Inject]
        private readonly InventoryService _inventoryService = null!;
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
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
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();

            foreach (InventoryItem tool in tools) {
                ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.Items.Find(toolItem => toolItem.Id == tool.Id);
                if (itemDescriptorModel == null) {
                    continue;
                }

                ToolsDescriptorModel? toolsDescriptorModel =
                        toolsDescriptor.Items.Find(toolItem => toolItem.Id == itemDescriptorModel.Id);
                if (toolsDescriptorModel == null) {
                    continue;
                }

                if (toolsDescriptorModel.ToolType == ToolType.WATER) {
                    result.Add(CreateItemSelectionModel(itemDescriptorModel.Icon, tool.Id, itemDescriptorModel.Name, token));
                }
            }

            if (result.Count == 0) {
                result.Add(CreateItemSelectionModel(baseIcon, string.Empty, "any water container", token));
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