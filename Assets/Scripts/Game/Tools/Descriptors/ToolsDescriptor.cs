using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Tools.Descriptors
{
    [CreateAssetMenu(fileName = "ToolsDescriptor", menuName = "Dacha/Descriptors/ToolsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ToolsDescriptor))]
    public class ToolsDescriptor : Descriptor<string, ToolsDescriptorModel>
    {
        
    }
}
