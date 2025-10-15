using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Harvest.Descriptor
{
    [CreateAssetMenu(fileName = "PlantHarvestDescriptor", menuName = "Dacha/Descriptors/PlantHarvestDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantHarvestDescriptor))]
    public class PlantHarvestDescriptor : Descriptor<string, PlantHarvestDescriptorModel>
    {
        
    }
}