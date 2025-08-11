using System;
using Game.Interactable.Model;
using UnityEngine;

namespace Game.Interactable.Descriptor
{
    [Serializable]
    public class InteractionDescriptorModel
    {
        [field: SerializeField]
        public InteractableType InteractableType { get; set; }
        
    }
}