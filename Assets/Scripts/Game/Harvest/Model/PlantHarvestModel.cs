namespace Game.Harvest.Model
{
    public class PlantHarvestModel
    {
        public string HarvestId { get; }
        public float Progress { get; set; }
        public HarvestGrowStage Stage { get; set; }
        
        public PlantHarvestModel(string harvestDescriptorId, float stageProgress, HarvestGrowStage currentStage)
        {
            HarvestId = harvestDescriptorId;
            Progress = stageProgress;
            Stage = currentStage;
        }
    }
}