using System;
using System.Collections.Generic;
using Game.Interactable.Model;
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