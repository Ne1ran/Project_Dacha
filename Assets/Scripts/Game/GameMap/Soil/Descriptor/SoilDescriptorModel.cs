using System;
using Game.GameMap.Soil.Model;
using UnityEngine;

namespace Game.GameMap.Soil.Descriptor
{
    [Serializable]
    public class SoilDescriptorModel
    {
        [field: SerializeField]
        public SoilType SoilType { get; set; }
        [field: SerializeField, Range(0f, 14f)]
        public float Ph { get; set; } = 6.5f;
        [field: SerializeField, Range(0f, 5f), Tooltip("Amount of salines in soil in percents. Max 5%")]
        public float Salinity { get; set; } = 0.1f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Changes breathe for roots. Also represent amount of water, that can stay in soil (100-Breathability)")]
        public float Breathability { get; set; } = 50f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Humus mass in percents")]
        public float Humus { get; set; } = 10f;
        [field: SerializeField, Tooltip("Soil mass in kilograms. Need for soil parameters calculations.")]
        public float Mass { get; set; } = 1000f;
        [field: SerializeField, Tooltip("Time for soil to recover to its starting values. If we remove half of its humus or salinity and won't change anything soil will recover to its starting values.")]
        public int RecoveryDays { get; set; } = 180;
        [field: SerializeField, Tooltip("Water amount in litres")]
        public float StartWaterAmount { get; set; } = 10f;
        [field: SerializeField]
        public SoilElementsDescriptorModel ElementsDescriptorModel { get; set; } = null!;
    }
}