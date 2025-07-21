using System.Collections.Generic;

namespace Game.Inventory.Model
{
    public class InventoryModel
    {
        public List<InventoryItem> InventoryItems { get; set; }

        public bool AddItem(InventoryItem item)
        {
            // todo neiran сделать проверку на тип предмета, если он может быть с чем-то стакнут - сделать так. Пока просто добавляем
            if (InventoryItems.Count >= Constants.Constants.INVENTORY_SLOTS) {
                return false;
            }
            
            InventoryItems.Add(item);
            return true;
        }

        public bool RemoveItem(InventoryItem item)
        {
            if (!InventoryItems.Contains(item)) {
                return false;
            }

            InventoryItems.Remove(item);
            return true;
        }
    }
}