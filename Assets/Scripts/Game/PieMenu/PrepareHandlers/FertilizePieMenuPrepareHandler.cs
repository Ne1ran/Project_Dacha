using System.Collections.Generic;
using System.Threading;
using Core.Conditions.Service;
using Core.Descriptors.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Fertilizers.Descriptor;
using Game.Interactable.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.PieMenu.Model;
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

        public UniTask<PieMenuItemModel> Prepare(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            List<PieMenuItemSelectionModel> selectionModels = UpdateSelectionModels();
            if (selectionModels.Count == 0) {
                selectionModels.Add(new(string.Empty, pieMenuSettings.BaseIcon, "any fertilizer"));
            }

            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionHandlerName, pieMenuSettings.Title, pieMenuSettings.Description, selectionModels);
            return UniTask.FromResult(itemModel);
        }

        private List<PieMenuItemSelectionModel> UpdateSelectionModels()
        {
            List<PieMenuItemSelectionModel> result = new();

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

                result.Add(new(fertilizer.Id, itemDescriptorModel.Icon, fertilizerDescriptorModel.Name));
            }

            return result;
        }
    }
}