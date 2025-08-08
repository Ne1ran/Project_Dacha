using System;
using Game.Soil.Model;
using UnityEngine;

namespace Game.Soil.Descriptor
{
    [Serializable]
    public class SoilDescriptorModel
    {
        [field: SerializeField]
        public SoilType SoilType { get; set; }
        [field: SerializeField]
        public float Ph { get; set; }
        [field: SerializeField]
        public int Salinity { get; set; }
        [field: SerializeField]
        public float Breathability { get; set; }
        [field: SerializeField]
        public float Humus { get; set; } = 30000;
    }
}