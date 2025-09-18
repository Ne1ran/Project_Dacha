using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Conditions.Service;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Interactable.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.PieMenu.Model;
using Game.Seeds.Descriptors;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("PrepareSowSeed")]
    public class SowSeedPieMenuPrepareHandler : IPieMenuPrepareHandler
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

            List<InventoryItem> seeds = _inventoryService.GetItemsByType(ItemType.SEED);
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();

            foreach (InventoryItem seedItem in seeds) {
                ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.ItemDescriptors.Find(seed => seed.ItemId == seedItem.Id);
                if (itemDescriptorModel == null) {
                    continue;
                }

                SeedsDescriptorModel? seedsDescriptorModel = seedsDescriptor.Items.Find(seed => seed.SeedId == itemDescriptorModel.ItemId);
                if (seedsDescriptorModel == null) {
                    continue;
                }

                result.Add(CreateItemSelectionModel(itemDescriptorModel.Icon, seedItem.Id, seedsDescriptorModel.SeedName, token));
            }

            if (result.Count == 0) {
                result.Add(CreateItemSelectionModel(baseIcon, string.Empty, "any seeds", token));
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