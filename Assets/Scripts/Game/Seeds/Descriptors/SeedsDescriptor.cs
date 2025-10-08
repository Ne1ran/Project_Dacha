using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Seeds.Descriptors
{
    [CreateAssetMenu(fileName = "SeedsDescriptor", menuName = "Dacha/Descriptors/SeedsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SeedsDescriptor))]
    public class SeedsDescriptor : Descriptor<string, SeedsDescriptorModel>
    {
        
    }
}