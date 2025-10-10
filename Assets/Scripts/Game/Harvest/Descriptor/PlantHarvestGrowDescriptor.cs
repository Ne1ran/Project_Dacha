using System;
using Core.Common.Descriptor;
using Game.Harvest.Model;
using UnityEngine;

namespace Game.Harvest.Descriptor
{
    [Serializable]
    public class PlantHarvestGrowDescriptor
    {
        [field: SerializeField, Tooltip("Harvest base quality (will be affected by different calculations and skills)")]
        public HarvestQuality CurrentQuality { get; set; } = HarvestQuality.None;
        [field: SerializeField, Tooltip("Harvest consumption from the soil on current stage")]
        public ConsumptionDescriptor HarvestConsumption { get; set; } = null!;
        [field: SerializeField, Tooltip("Time in days for harvest to grow into new stage")]
        public float NextStageGrowDays { get; set; } = 7f;
    }
}