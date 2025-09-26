using System.Collections.Generic;
using Game.GameMap.Soil.Model;
using Game.Stress.Model;

namespace Game.Plants.Model
{
    public class PlantGrowCalculationModel
    {
        public float GrowMultiplier { get; set; } = 1f;
        public float Damage { get; set; }
        public Dictionary<StressType, float> Stress { get; set; } = new();
        public bool BlockGrowth { get; set; }
        public bool BlockHealing { get; set; }
        public bool BlockImmunityGain { get; set; }

        public ElementsModel Elements { get; set; } = null!;
        public float Water { get; set; }
    }
}