using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEngine;

namespace Game.Symptoms.Descriptor
{
    [CreateAssetMenu(fileName = "SymptomsDescriptor", menuName = "Dacha/Descriptors/SymptomsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SymptomsDescriptor))]
    public class SymptomsDescriptor : Descriptor<string, SymptomDescriptorModel>
    {
       
    }
}