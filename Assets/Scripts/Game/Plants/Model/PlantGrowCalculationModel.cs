using System.Collections.Generic;
using Game.Soil.Model;
using Game.Stress.Model;

namespace Game.Plants.Model
{
    public class PlantGrowCalculationModel
    {
        public float ActualGrowth { get; set; }
        public float GrowMultiplier { get; set; } = 1f;
        public float Damage { get; set; }
        public Dictionary<StressType, float> Stress { get; set; } = new();
        public bool BlockGrowth { get; set; }
        public bool BlockHealing { get; set; }
        public bool BlockImmunityGain { get; set; }
        public bool BlockHarvestGrowth { get; set; }

        public SoilConsumptionModel Consumption { get; set; } = null!;
    }
}