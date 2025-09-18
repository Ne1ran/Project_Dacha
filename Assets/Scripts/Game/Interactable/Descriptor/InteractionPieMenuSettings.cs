using System;
using System.Collections.Generic;
using Core.Conditions.Descriptor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Interactable.Descriptor
{
    [Serializable]
    public class InteractionPieMenuSettings
    {
        [field: SerializeField]
        public string InteractionHandlerName { get; set; } = string.Empty;
        [field: SerializeField]
        public string PrepareHandlerName { get; set; } = "Simple";
        [field: SerializeField]
        public string Title { get; set; } = string.Empty;
        [field: SerializeField]
        public string Description { get; set; } = string.Empty;
        [field: SerializeField]
        public AssetReference? BaseIcon { get; set; }
        [field: SerializeField]
        [TableList]
        public List<ConditionDescriptor> Conditions { get; set; } = new();
    }
}