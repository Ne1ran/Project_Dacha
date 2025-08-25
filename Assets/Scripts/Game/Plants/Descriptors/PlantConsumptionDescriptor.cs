using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantConsumptionDescriptor
    {
        [field: SerializeField, Tooltip("Need to do calculation?")]
        public bool Ignore { get; set; }
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of nitrogen elements NO3 for plant in grams")]
        public float NitrogenUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of potassium elements K for plant in grams")]
        public float PotassiumUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of phosphorus elements P2O5 for plant in grams")]
        public float PhosphorusUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for plant growth if enough elements")]
        public float GrowBuff { get; set; } = 0.05f;
        [field: SerializeField, Range(-2f, 0f), Tooltip("Debuff for plant growth if not enough elements")]
        public float GrowDebuff { get; set; } = -0.15f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of water needed for plant in grams")]
        public float WaterUsage { get; set; } = 5f;
    }
}