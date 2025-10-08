using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Harvest.Descriptor
{
    [CreateAssetMenu(fileName = "PlantHarvestDescriptor", menuName = "Dacha/Descriptors/PlantHarvestDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantHarvestDescriptor))]
    public class PlantHarvestDescriptor : Descriptor<string, PlantHarvestDescriptorModel>
    {
        
    }
}