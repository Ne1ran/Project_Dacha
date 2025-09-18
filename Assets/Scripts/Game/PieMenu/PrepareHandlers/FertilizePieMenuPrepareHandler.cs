using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Conditions.Service;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Fertilizers.Descriptor;
using Game.Interactable.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.PieMenu.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("Fertilize")]
    public class FertilizePieMenuPrepareHandler : IPieMenuPrepareHandler
    {
        [Inject]
        private readonly InventoryService _inventoryService = null!;
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
        [Inject]
        private readonly ConditionService _conditionService = null!;
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

            List<InventoryItem> fertilizers = _inventoryService.GetItemsByType(ItemType.FERTILIZER);
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            FertilizersDescriptor fertilizersDescriptor = _descriptorService.Require<FertilizersDescriptor>();

            foreach (InventoryItem fertilizer in fertilizers) {
                ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.ItemDescriptors.Find(fert => fert.ItemId == fertilizer.Id);
                if (itemDescriptorModel == null) {
                    continue;
                }

                FertilizerDescriptorModel? fertilizerDescriptorModel =
                        fertilizersDescriptor.Fertilizers.Find(fert => fert.Id == itemDescriptorModel.ItemId);
                if (fertilizerDescriptorModel == null) {
                    continue;
                }

                result.Add(CreateItemSelectionModel(itemDescriptorModel.Icon, fertilizer.Id, fertilizerDescriptorModel.Name, token));
            }

            if (result.Count == 0) {
                result.Add(CreateItemSelectionModel(baseIcon, string.Empty, "any fertilizer", token));
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