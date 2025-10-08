using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Symptoms.Descriptor
{
    [CreateAssetMenu(fileName = "SymptomsDescriptor", menuName = "Dacha/Descriptors/SymptomsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SymptomsDescriptor))]
    public class SymptomsDescriptor : Descriptor<string, SymptomDescriptorModel>
    {
       
    }
}