using System;
using Game.Difficulty.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Temperature.Descriptor
{
    [Serializable]
    public class TemperatureDistributionModelDescriptor
    {
        [field: SerializeField]
        public DachaPlaceType PlaceType { get; set; } = DachaPlaceType.None;
        
        [field: SerializeField]
        public SerializedDictionary<int, float> TemperatureDistribution { get; set; } = new();
    }
}