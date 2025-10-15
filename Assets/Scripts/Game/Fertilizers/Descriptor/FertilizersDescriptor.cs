using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Fertilizers.Descriptor
{
    [CreateAssetMenu(fileName = "FertilizersDescriptor", menuName = "Dacha/Descriptors/FertilizersDescriptor")]
    [Descriptor("Descriptors/" + nameof(FertilizersDescriptor))]
    public class FertilizersDescriptor : Descriptor<string, FertilizerDescriptorModel>
    {
    }
}