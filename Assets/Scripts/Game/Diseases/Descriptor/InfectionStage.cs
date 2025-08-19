using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class InfectionStage
    {
        [field: SerializeField, Tooltip("Infection stage of a plant")]
        public int Stage { get; set; } = 1;
        [field: SerializeField, Tooltip("Damage dealt to plant everyday if infection is on current stage"), Range(0f, 100f)]
        public float PlantDamagePerDay { get; set; } = 1f;
        [field: SerializeField, Tooltip("Random symptoms that player can see to identify disease")]
        [TableList]
        public List<string> RandomSymptoms { get; set; } = new();
        [field: SerializeField, Tooltip("Total amount of growth points to change stage"), Range(0f, 100f)]
        public float StageGrowth { get; set; } = 25f;
        [field: SerializeField, Tooltip("Base speed of infection growth of current stage per day"), Range(0f, 100f)]
        public float BaseGrowthPerDay { get; set; } = 1f;
    }
}