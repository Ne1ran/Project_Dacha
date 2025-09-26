using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantConsumptionDescriptor
    {
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of nitrogen elements NO3 for plant in grams")]
        public float NitrogenUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of potassium elements K for plant in grams")]
        public float PotassiumUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of phosphorus elements P2O5 for plant in grams")]
        public float PhosphorusUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of water needed for plant in grams")]
        public float WaterUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Multiplier for all elements. If soil contains enough, plant will receive grow buff")]
        public float PreferredUsageMultiplier { get; set; } = 10f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for plant growth if enough elements")]
        public float GrowBuff { get; set; } = 0.05f;
        [field: SerializeField, Range(-2f, 0f), Tooltip("Stress gain for plant if not enough elements")]
        public float StressGain { get; set; } = 5f;
    }
}