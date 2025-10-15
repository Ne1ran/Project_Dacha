using Core.Attributes;
using Game.Inventory.Service;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Utils;

namespace Game.Items.Service
{
    [Service]
    public class PickUpItemService
    {
        private readonly ItemsDescriptor _itemsDescriptor;
        private readonly InventoryService _inventoryService;

        public PickUpItemService(InventoryService inventoryService,
                                 ItemsDescriptor itemsDescriptor)
        {
            _inventoryService = inventoryService;
            _itemsDescriptor = itemsDescriptor;
        }

        public void PickUpItem(ItemController itemController)
        {
            string itemId = itemController.ItemId;
            ItemDescriptorModel itemModel = _itemsDescriptor.Require(itemId);
            if (_inventoryService.TryAddItemToInventory(itemId, itemModel.Type)) {
                itemController.gameObject.DestroyObject();
            }
        }
    }
}