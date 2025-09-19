using System.Collections.Generic;
using Core.Attributes;
using Game.Interactable.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Interactable.Descriptor
{
    [CreateAssetMenu(fileName = "InteractionDescriptor", menuName = "Dacha/Descriptors/InteractionDescriptor")]
    [Descriptor("Descriptors/" + nameof(InteractionDescriptor))]
    public class InteractionDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<InteractionDescriptorModel> Items { get; private set; } = new();

        public InteractionDescriptorModel RequireByType(InteractableType soilType)
        {
            InteractionDescriptorModel? descriptorModel = Items.Find(desc => desc.InteractableType == soilType);
            if (descriptorModel == null) {
                throw new KeyNotFoundException($"Interaction descriptor not found with type={soilType}");
            }

            return descriptorModel;
        }
    }
}