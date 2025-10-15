using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Common.Descriptors
{
    [CreateAssetMenu(fileName = "SelectorsDescriptor", menuName = "Dacha/Descriptors/SelectorsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SelectorsDescriptor))]
    public class SelectorsDescriptor : Descriptor<string, SelectorDescriptorModel>
    {
        
    }
}