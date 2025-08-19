using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class DiseaseInfectionModel
    {
        [field: SerializeField]
        public float StartingChance { get; set; } = 0.001f;
        [field: SerializeField]
        [TableList]
        public List<TileRangePair> TileRangeMultipliers { get; set; } = new();
        [field: SerializeField]
        [TableList]
        public List<InfectionStage> InfectionStages { get; set; } = new();
    }
}