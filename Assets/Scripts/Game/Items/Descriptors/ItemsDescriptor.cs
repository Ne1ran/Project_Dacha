using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Items.Descriptors
{
    [CreateAssetMenu(fileName = "ItemsDescriptor", menuName = "Dacha/Descriptors/ItemsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ItemsDescriptor))]
    public class ItemsDescriptor : Descriptor<string, ItemDescriptorModel>
    {
        
    }
}