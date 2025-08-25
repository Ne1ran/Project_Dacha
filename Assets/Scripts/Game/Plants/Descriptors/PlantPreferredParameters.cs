using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantPreferredParameters
    {
        [field: SerializeField, Range(350f, 3500f), Tooltip("Amount of sunlight needed for plant to grow")]
        public float Sunlight { get; set; } = 500f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Min temperature needed for plant to grow")]
        public float MinTemperature { get; set; } = 15f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Max temperature needed for plant to grow")]
        public float MaxTemperature { get; set; } = 35f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for plant")]
        public float PreferredTemperature { get; set; } = 27f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Min soil humidity preferred by plant")]
        public float MinSoilHumidity { get; set; } = 20f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Max soil humidity preferred by plant")]
        public float MaxSoilHumidity { get; set; } = 65f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Min air humidity preferred by plant")]
        public float MinAirHumidity { get; set; } = 20f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Max air humidity preferred by plant")]
        public float MaxAirHumidity { get; set; } = 65f;
    }
}