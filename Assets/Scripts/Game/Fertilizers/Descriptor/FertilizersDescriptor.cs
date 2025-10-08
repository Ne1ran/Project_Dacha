using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Fertilizers.Descriptor
{
    [CreateAssetMenu(fileName = "FertilizersDescriptor", menuName = "Dacha/Descriptors/FertilizersDescriptor")]
    [Descriptor("Descriptors/" + nameof(FertilizersDescriptor))]
    public class FertilizersDescriptor : Descriptor<string, FertilizerDescriptorModel>
    {
    }
}