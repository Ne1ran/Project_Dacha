using System;
using Game.Plants.Model;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantPhParameters : IPlantParameters
    {
        public PlantParametersType ParametersType => PlantParametersType.Ph;
        
        [field: SerializeField, Range(0f, 14f), Tooltip("Min ph for plant to live")]
        public float Min { get; set; } = 5f;
        [field: SerializeField, Range(0f, 14f), Tooltip("Max ph for plant to live")]
        public float Max { get; set; } = 7.5f;
        [field: SerializeField, Range(0f, 14f), Tooltip("Min preferred ph for plant")]
        public float MinPreferred { get; set; } = 6.5f;
        [field: SerializeField, Range(0f, 14f), Tooltip("Max preferred ph for plant")]
        public float MaxPreferred { get; set; } = 7.25f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 Ph deviation")]
        public float DamagePerDeviation { get; set; } = 10f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Stress gain per 1 ph deviation")]
        public float StressGain { get; set; } = 5f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred Ph")]
        public float GrowBuff { get; set; } = 0.05f;
    }
}