using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Plants.Descriptors
{
    [CreateAssetMenu(fileName = "PlantsDescriptor", menuName = "Dacha/Descriptors/PlantsDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantsDescriptor))]
    public class PlantsDescriptor : Descriptor<string, PlantsDescriptorModel>
    {
        
    }
}