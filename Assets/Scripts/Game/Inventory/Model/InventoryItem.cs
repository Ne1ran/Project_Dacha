namespace Game.Inventory.Model
{
    public class InventoryItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public ItemType ItemType { get; set; }
        public bool IsHotkey { get; set; }
    }
}