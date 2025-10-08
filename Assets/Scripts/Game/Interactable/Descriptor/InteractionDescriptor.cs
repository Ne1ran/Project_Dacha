using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Interactable.Model;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Interactable.Descriptor
{
    [CreateAssetMenu(fileName = "InteractionDescriptor", menuName = "Dacha/Descriptors/InteractionDescriptor")]
    [Descriptor("Descriptors/" + nameof(InteractionDescriptor))]
    public class InteractionDescriptor : Descriptor<InteractableType, InteractionDescriptorModel>
    {
        
    }
}