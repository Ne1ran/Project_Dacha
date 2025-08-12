using System;
using UnityEngine;

namespace Game.Interactable.Descriptor
{
    [Serializable]
    public class InteractionPieMenuSettings
    {
        [field: SerializeField]
        public string Title { get; set; } = null!;
        [field: SerializeField]
        public string Description { get; set; } = null!;
        [field: SerializeField]
        public string IconPath { get; set; } = null!;
    }
}