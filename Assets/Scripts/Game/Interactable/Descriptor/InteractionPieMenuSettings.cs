using System;
using System.Collections.Generic;
using Core.Conditions.Descriptor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Interactable.Descriptor
{
    [Serializable]
    public class InteractionPieMenuSettings
    {
        [field: SerializeField]
        public string InteractionName { get; set; } = string.Empty;
        [field: SerializeField]
        public string HandlerName { get; set; } = "Simple";
        [field: SerializeField]
        public string Title { get; set; } = string.Empty;
        [field: SerializeField]
        public string Description { get; set; } = string.Empty;
        [field: SerializeField]
        public string IconPath { get; set; } = string.Empty;
        [field: SerializeField]
        public string? NonConditionActionHandler { get; set; }
        [field: SerializeField]
        [TableList]
        public List<ConditionDescriptor> Conditions { get; set; } = new();
    }
}