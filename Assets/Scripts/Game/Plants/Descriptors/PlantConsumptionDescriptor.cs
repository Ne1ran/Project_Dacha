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
        [field: SerializeField, Range(0f, 10f), Tooltip("Stress gain for plant if not enough elements")]
        public float StressGainNitrogen { get; set; } = 1f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Stress gain for plant if not enough elements")]
        public float StressGainPotassium { get; set; } = 1f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Stress gain for plant if not enough elements")]
        public float StressGainPhosphorus { get; set; } = 1f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Stress gain for plant if not enough elements")]
        public float StressGainWater { get; set; } = 2.5f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Stress gain for plant if no elements and water at all")]
        public float NotEnoughStressGain { get; set; } = 5f;
    }
}