using System;
using UnityEngine;

namespace Game.Descriptors
{
    [Serializable]
    public class ToolsDescriptorModel
    {
        [field: SerializeField]
        public string ToolId { get; set; } = null!;
        [field: SerializeField]
        public string ToolPrefab { get; set; } = null!;
    }
}