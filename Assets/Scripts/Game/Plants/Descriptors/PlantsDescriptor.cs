using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [CreateAssetMenu(fileName = "PlantsDescriptor", menuName = "Dacha/Descriptors/PlantsDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantsDescriptor))]
    public class PlantsDescriptor : Descriptor<string, PlantsDescriptorModel>
    {
        
    }
}