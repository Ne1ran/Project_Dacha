using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Descriptors
{
    [Serializable]
    public class ToolsDescriptorModel
    {
        [field: SerializeField]
        public string ToolId { get; set; } = null!;
        [field: SerializeField]
        public string ToolPrefab { get; set; } = null!;
        [field: SerializeField]
        public Image ToolIcon { get; set; } = null!;
    }
}