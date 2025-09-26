using System;
using UnityEngine;

namespace Game.GameMap.Soil.Descriptor
{
    [Serializable]
    public class SoilElementsDescriptorModel
    {
        [field: SerializeField, Range(0f, 750f), Tooltip("Amount of nitrogen elements NO3 in soil in grams")]
        public float StartNitrogen { get; set; } = 100f;
        [field: SerializeField, Range(0f, 750f), Tooltip("Amount of potassium elements K in soil in grams")]
        public float StartPotassium { get; set; } = 100f;
        [field: SerializeField, Range(0f, 500f), Tooltip("Amount of phosphorus elements P2O5 in soil in grams")]
        public float StartPhosphorus { get; set; } = 70f;
    }
}