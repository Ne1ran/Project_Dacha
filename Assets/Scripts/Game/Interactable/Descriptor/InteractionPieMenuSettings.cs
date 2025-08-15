using System;
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
    }
}