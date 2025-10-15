namespace Game.Harvest.Model
{
    public class HarvestModel
    {
        public string HarvestInventoryItemId { get; }
        public HarvestQuality HarvestQuality { get; }
        public int Amount { get; }

        public HarvestModel(string harvestInventoryItemId, HarvestQuality harvestQuality, int amount)
        {
            HarvestInventoryItemId = harvestInventoryItemId;
            HarvestQuality = harvestQuality;
            Amount = amount;
        }
    }
}