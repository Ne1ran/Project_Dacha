using Game.Inventory.Model;

namespace Game.Items.Model
{
    public class ItemModel
    {
        public string ItemId { get; }
        public string? ItemPrefab { get; }
        public ItemType ItemType { get; }
        public bool Stackable { get; } = false;
        public bool ShowInHand { get; } = false;
        public int MaxStack { get; }

        public ItemModel(string itemId, string? itemPrefab, ItemType itemType, bool stackable, bool showInHand, int maxStack)
        {
            ItemId = itemId;
            ItemPrefab = itemPrefab;
            ItemType = itemType;
            Stackable = stackable;
            ShowInHand = showInHand;
            MaxStack = maxStack;
        }
    }
}