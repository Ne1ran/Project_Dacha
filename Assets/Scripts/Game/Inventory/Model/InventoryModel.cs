using System.Collections.Generic;
using System.Linq;

namespace Game.Inventory.Model
{
    public class InventoryModel
    {
        public List<InventorySlot> InventorySlots { get; }

        public InventoryModel(List<InventorySlot> inventorySlots)
        {
            InventorySlots = inventorySlots;
        }

        public bool AddItem(InventoryItem item)
        {
            // todo neiran сделать проверку на тип предмета, если он может быть с чем-то стакнут - сделать так. Пока просто добавляем

            foreach (InventorySlot slot in InventorySlots) {
                if (slot.InventoryItem != null) {
                    continue;
                }

                slot.InventoryItem = item;
                return true;
            }

            return false;
        }

        public bool RemoveItem(InventoryItem item)
        {
            foreach (InventorySlot inventorySlot in InventorySlots) {
                if (inventorySlot.InventoryItem != item) {
                    continue;
                }
                
                inventorySlot.InventoryItem = null;
                return true;
            }

            return false;
        }

        public bool RemoveItem(int slotIndex)
        {
            InventorySlots[slotIndex + 1].InventoryItem = null;
            return true;
        }

        public int OccupiedSlots
        {
            get
            {
                int counter = 0;
                InventorySlots.ForEach(slot => {
                    if (slot.InventoryItem != null) {
                        counter++;
                    }
                });

                return counter;
            }
        }

        public bool HasFreeSpace
        {
            get
            {
                foreach (InventorySlot slot in InventorySlots) {
                    if (slot.InventoryItem == null) {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}