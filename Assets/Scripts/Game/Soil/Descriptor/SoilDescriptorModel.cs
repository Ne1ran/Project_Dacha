using System;
using Game.Soil.Model;
using UnityEngine;

namespace Game.Soil.Descriptor
{
    [Serializable]
    public class SoilDescriptorModel
    {
        [field: SerializeField, Range(0f, 14f)]
        public float Ph { get; set; } = 6.5f;
        [field: SerializeField, Range(0f, 5f), Tooltip("Amount of salines in soil in percents. Max 5%")]
        public float Salinity { get; set; } = 0.1f;
        [field: SerializeField, Range(0f, 1f),
                Tooltip("Changes breathe for roots. Also represent amount of water, that can stay in soil (100-Breathability)")]
        public float Breathability { get; set; } = 0.5f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Humus mass in percents")]
        public float Humus { get; set; } = 0.1f;
        [field: SerializeField, Tooltip("Soil mass in kilograms. Need for soil parameters calculations.")]
        public float Mass { get; set; } = 1000f;
        [field: SerializeField, Tooltip("Time for soil to recover its normal values. Applies to humus, salinity, breathability and ph")]
        public int RecoveryDays { get; set; } = 730;
        [field: SerializeField, Tooltip("Usable water amount on start")]
        public float StartWaterAmount { get; set; } = 50f;
        [field: SerializeField]
        public SoilElementsDescriptorModel ElementsDescriptorModel { get; set; } = null!;
        [field: SerializeField]
        public SoilVisualDescriptor SoilVisualDescriptor { get; set; } = null!;
    }
}