using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Descriptors
{
    [CreateAssetMenu(fileName = "ToolsDescriptor", menuName = "Dacha/Descriptors/ToolsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ToolsDescriptor))]
    public class ToolsDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<ToolsDescriptorModel> ToolsDescriptors { get; private set; } = new();
    }
}
