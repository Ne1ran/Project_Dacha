using System;
using UnityEngine;

namespace Game.Fertilizers.Descriptor
{
    [Serializable]
    public class FertilizerElementsDescriptorModel
    {
        [field: SerializeField]
        public float NitrogenPercent { get; set; } = 15f;
        [field: SerializeField]
        public float PotassiumPercent { get; set; } = 15f;
        [field: SerializeField]
        public float PhosphorusPercent { get; set; } = 15f;
    }
}