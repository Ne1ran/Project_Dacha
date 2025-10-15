using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Seeds.Descriptors
{
    [CreateAssetMenu(fileName = "SeedsDescriptor", menuName = "Dacha/Descriptors/SeedsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SeedsDescriptor))]
    public class SeedsDescriptor : Descriptor<string, SeedsDescriptorModel>
    {
        
    }
}