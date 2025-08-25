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
using Game.Seeds.Descriptors;
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

        public UniTask<PieMenuItemModel> Prepare(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            List<PieMenuItemSelectionModel> selectionModels = UpdateSelectionModels();
            if (selectionModels.Count == 0) {
                selectionModels.Add(new(string.Empty, pieMenuSettings.BaseIcon, "any seeds"));
            }

            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionHandlerName, pieMenuSettings.Title, pieMenuSettings.Description, selectionModels);
            return UniTask.FromResult(itemModel);
        }

        private List<PieMenuItemSelectionModel> UpdateSelectionModels()
        {
            List<PieMenuItemSelectionModel> result = new();

            List<InventoryItem> seeds = _inventoryService.GetItemsByType(ItemType.SEED);
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();

            foreach (InventoryItem seedItem in seeds) {
                ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.ItemDescriptors.Find(seed => seed.ItemId == seedItem.Id);
                if (itemDescriptorModel == null) {
                    continue;
                }

                SeedsDescriptorModel? seedsDescriptorModel =
                        seedsDescriptor.Items.Find(seed => seed.SeedId == itemDescriptorModel.ItemId);
                if (seedsDescriptorModel == null) {
                    continue;
                }

                result.Add(new(seedItem.Id, itemDescriptorModel.Icon, seedsDescriptorModel.SeedName));
            }

            return result;
        }
    }
}