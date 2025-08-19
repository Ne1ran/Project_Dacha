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
        [field: SerializeField, Tooltip("Amount of days needed for saved infection to be removed")]
        public int DisposeDaysNeeded { get; set; } = 700;
        [field: SerializeField, Tooltip("Amount of crop rotations needed for saved infection to be removed")]
        public int DisposeCropRotationNeeded { get; set; } = 3;
        [field: SerializeField]
        [TableList]
        public List<TileRangePair> TileRangeMultipliers { get; set; } = new();
        [field: SerializeField]
        [TableList]
        public List<InfectionStage> InfectionStages { get; set; } = new();
    }
}