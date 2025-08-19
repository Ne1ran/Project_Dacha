using System;
using UnityEngine;

namespace Game.Tools.Descriptors
{
    [Serializable]
    public class ToolsDescriptorModel
    {
        [field: SerializeField]
        public string ToolId { get; set; } = null!;
        [field: SerializeField]
        public string ToolPrefab { get; set; } = null!;
        [field: SerializeField]
        public string UseHandler { get; set; } = null!;
        [field: SerializeField]
        public bool CanCarryInfection { get; set; }
    }
}