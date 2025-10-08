using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Difficulty.Model;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Temperature.Descriptor
{
    
    [CreateAssetMenu(fileName = "TemperatureDistributionDescriptor", menuName = "Dacha/Descriptors/TemperatureDistributionDescriptor")]
    [Descriptor("Descriptors/" + nameof(TemperatureDistributionDescriptor))]
    public class TemperatureDistributionDescriptor : Descriptor<DachaPlaceType, SerializedDictionary<int, float>>
    {
        
    }
}