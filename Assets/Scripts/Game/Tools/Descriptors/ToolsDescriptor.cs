using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Tools.Descriptors
{
    [CreateAssetMenu(fileName = "ToolsDescriptor", menuName = "Dacha/Descriptors/ToolsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ToolsDescriptor))]
    public class ToolsDescriptor : Descriptor<string, ToolsDescriptorModel>
    {
        
    }
}
