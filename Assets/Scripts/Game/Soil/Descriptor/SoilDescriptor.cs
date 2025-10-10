using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Soil.Model;
using UnityEngine;

namespace Game.Soil.Descriptor
{
    [CreateAssetMenu(fileName = "SoilDescriptor", menuName = "Dacha/Descriptors/SoilDescriptor")]
    [Descriptor("Descriptors/" + nameof(SoilDescriptor))]
    public class SoilDescriptor : Descriptor<SoilType, SoilDescriptorModel>
    {
        
    }
}