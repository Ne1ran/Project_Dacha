using System;
using UnityEngine;

namespace Core.Common.Descriptor
{
    [Serializable]
    public class ConsumptionDescriptor
    {
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of nitrogen elements NO3 in grams")]
        public float NitrogenUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of potassium elements K in grams")]
        public float PotassiumUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of phosphorus elements P2O5 in grams")]
        public float PhosphorusUsage { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Amount of water needed in grams")]
        public float WaterUsage { get; set; } = 5f;
    }
}