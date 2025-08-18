using Core.Conditions.Checker;
using Core.Parameters;
using Game.Common.Handlers;
using Game.Inventory.Model;
using Game.Inventory.Service;
using UnityEngine;
using VContainer;

namespace Game.Conditions.Checkers
{
    [Handler("hasInventoryItemCondition")]
    public class HasInventoryItemCondition : Condition
    {
        [Inject]
        private readonly InventoryService _inventoryService = null!;

        private ItemType _itemType;
        private string? _itemId;

        protected override void Initialize(Parameters parameters)
        {
            _itemType = parameters.Get<ItemType>(ParameterNames.ItemType);
            _itemId = parameters.Get<string>(ParameterNames.ItemId);
        }

        protected override ConditionResult CheckInternal()
        {
            bool hasType = _itemType != ItemType.NONE;
            bool hasId = !string.IsNullOrEmpty(_itemId);

            if (hasType && hasId) {
                return new(_inventoryService.HasItemByTypeAndId(_itemType, _itemId!), string.Empty);
            }

            if (hasId) {
                return new(_inventoryService.HasItemById(_itemId!), string.Empty);
            }

            if (hasType) {
                return new(_inventoryService.HasItemByType(_itemType), string.Empty);
            }

            Debug.LogWarning("Condition does not have any parameters about items! Will return false.");
            return new(false, string.Empty);
        }
    }
}