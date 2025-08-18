using System.Collections.Generic;
using System.Threading;
using Core.Conditions.Checker;
using Core.Conditions.Service;
using Core.Descriptors.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Fertilizers.Descriptor;
using Game.Interactable.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.PieMenu.ActionHandler;
using Game.PieMenu.Model;
using Game.PieMenu.Service;
using UnityEngine;
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
        private readonly PieMenuActionFactory _actionFactory = null!;

        public UniTask<PieMenuItemModel> Prepare(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            Sprite? sprite = Resources.Load<Sprite>(pieMenuSettings.IconPath); // todo neiran remove when go to addressables!!!
            List<PieMenuItemSelectionModel> selectionModels = new();
            ConditionResult conditionResult = _conditionService.Check(pieMenuSettings.Conditions);

            IPieMenuActionHandler? actionHandler = null;
            if (conditionResult.IsAllowed) {
                UpdateSelectionModels(selectionModels);
            } else {
                string? nonConditionActionHandler = pieMenuSettings.NonConditionActionHandler;
                if (!string.IsNullOrEmpty(nonConditionActionHandler)) {
                    actionHandler = _actionFactory.Create(nonConditionActionHandler);
                }
            }

            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionName, pieMenuSettings.Title, pieMenuSettings.Description, sprite,
                                             selectionModels, actionHandler);
            return UniTask.FromResult(itemModel);
        }

        private void UpdateSelectionModels(List<PieMenuItemSelectionModel> selectionModels)
        {
            List<InventoryItem> fertilizers = _inventoryService.GetItemsByType(ItemType.FERTILIZER);
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            FertilizersDescriptor fertilizersDescriptor = _descriptorService.Require<FertilizersDescriptor>();
                
            foreach (InventoryItem fertilizer in fertilizers) {
                ItemDescriptorModel? itemDescriptorModel = itemsDescriptor.ItemDescriptors.Find(fert => fert.ItemId == fertilizer.Id);
                if (itemDescriptorModel == null) {
                    continue;
                }

                FertilizerDescriptorModel? fertilizerDescriptorModel = fertilizersDescriptor.Fertilizers.Find(fert => fert.Id == fertilizer.Id);
                if (fertilizerDescriptorModel == null) {
                    continue;
                }

                selectionModels.Add(new(fertilizer.Id, itemDescriptorModel.Icon, fertilizerDescriptorModel.Name));
            }
        }
    }
}