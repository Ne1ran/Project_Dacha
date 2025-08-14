using System;
using UnityEngine;

namespace Game.GameMap.Soil.Descriptor
{
    [Serializable]
    public class SoilElementsDescriptorModel
    {
        [field: SerializeField, Range(75f, 750f), Tooltip("Amount of nitrogen elements NO3 in soil in grams")]
        public float StartNitrogen { get; set; } = 350f;
        [field: SerializeField, Range(50f, 750f), Tooltip("Amount of potassium elements K in soil in grams")]
        public float StartPotassium { get; set; } = 300f;
        [field: SerializeField, Range(30f, 500f), Tooltip("Amount of phosphorus elements P2O5 in soil in grams")]
        public float StartPhosphorus { get; set; } = 250f;
    }
}