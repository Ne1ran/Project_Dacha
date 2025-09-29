using Core.Attributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Symptoms.Descriptor
{
    [CreateAssetMenu(fileName = "SymptomsDescriptor", menuName = "Dacha/Descriptors/SymptomsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SymptomsDescriptor))]
    public class SymptomsDescriptor : ScriptableObject
    {
        [field: SerializeField]
        public SerializedDictionary<string, SymptomDescriptorModel> Items { get; set; } = new();
    }
}