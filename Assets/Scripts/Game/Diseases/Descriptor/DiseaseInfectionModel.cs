using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class DiseaseInfectionModel
    {
        [field: SerializeField, Tooltip("Starting chance of infection per day in percents")]
        public float StartingChance { get; set; } = 0.001f;
        [field: SerializeField, Tooltip("Amount of days needed for saved infection to be removed from soil")]
        public int DisposeDaysNeeded { get; set; } = 700;
        [field: SerializeField, Tooltip("Amount of crop rotations needed for saved infection to be removed from soil")]
        public int DisposeCropRotationNeeded { get; set; } = 3;

        [field: SerializeField, Range(0f, 100f), Tooltip("Low immunity for infection")]
        public float LowImmunity { get; set; } = 5.0f;
        [field: SerializeField, Range(0f, 100f), Tooltip("High immunity for infection")]
        public float HighImmunity { get; set; } = 50.0f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Low health for infection")]
        public float LowHealth { get; set; } = 20.0f;
        [field: SerializeField, Range(0f, 100f), Tooltip("High health for infection")]
        public float HighHealth { get; set; } = 80.0f;

        [field: SerializeField, Range(0.1f, 5f), Tooltip("Infection multiplier if low immunity")]
        public float LowImmunityMultiplier { get; set; } = 2.0f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Infection multiplier if high immunity")]
        public float HighImmunityMultiplier { get; set; } = 0.5f;
        [field: SerializeField, Range(0.1f, 5f), Tooltip("Infection multiplier if low health")]
        public float LowHealthMultiplier { get; set; } = 3.0f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Infection multiplier if high health")]
        public float HighHealthMultiplier { get; set; } = 0.5f;
        [field: SerializeField]
        public InfectionGrowthDescriptor GrowthDescriptor { get; set; } = null!;

        [field: SerializeField]
        [TableList]
        public List<TileRangePair> TileRangeMultipliers { get; set; } = new();
        [field: SerializeField]
        [TableList]
        public List<InfectionStage> InfectionStages { get; set; } = new();

        public int GetMaxTileRange()
        {
            int range = 0;
            foreach (TileRangePair tileRangePair in TileRangeMultipliers) {
                if (tileRangePair.TileRange < range) {
                    continue;
                }

                range = tileRangePair.TileRange;
            }

            return range;
        }
    }
}