using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Interactable.Descriptor
{
    [Serializable]
    public class InteractionDescriptorModel
    {
        [field: SerializeField]
        [TableList]
        public List<InteractionPieMenuSettings> Settings { get; set; } = null!;
    }
}