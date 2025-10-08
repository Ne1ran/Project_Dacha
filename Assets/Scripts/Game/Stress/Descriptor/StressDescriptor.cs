using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Stress.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Stress.Descriptor
{
    [CreateAssetMenu(fileName = "StressDescriptor", menuName = "Dacha/Descriptors/StressDescriptor")]
    [Descriptor("Descriptors/" + nameof(StressDescriptor))]
    public class StressDescriptor : Descriptor<StressType, StressModelDescriptor>
    {
       
    }
}