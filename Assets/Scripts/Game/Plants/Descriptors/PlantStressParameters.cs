using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantStressParameters
    {
        [field: SerializeField, Range(0f, 200f), Tooltip("Maximum stress plant can get")]
        public float MaxStress { get; set; } = 100f;
        [field: SerializeField, Tooltip("Plant stress decrease per day")]
        public float DailyStressDecrease { get; set; } = 5f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Threshold from max stress to block plant healing")]
        public float BlockHealingThreshold { get; set; } = 0.25f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Threshold from max stress to block plant immunity gain")]
        public float BlockImmunityGainThreshold { get; set; } = 0.5f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Threshold from max stress to block plant growth")]
        public float BlockGrowthThreshold { get; set; } = 0.33f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Threshold from max stress to deal stress damage")]
        public float DealDamageThreshold { get; set; } = 0.6f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Multiplier for current stress to deal damage")]
        public float StressDamageMultiplier { get; set; } = 0.05f;
    }
}