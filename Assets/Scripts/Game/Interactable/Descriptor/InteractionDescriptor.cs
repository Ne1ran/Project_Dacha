using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Interactable.Model;
using UnityEngine;

namespace Game.Interactable.Descriptor
{
    [CreateAssetMenu(fileName = "InteractionDescriptor", menuName = "Dacha/Descriptors/InteractionDescriptor")]
    [Descriptor("Descriptors/" + nameof(InteractionDescriptor))]
    public class InteractionDescriptor : Descriptor<InteractableType, InteractionDescriptorModel>
    {
        
    }
}