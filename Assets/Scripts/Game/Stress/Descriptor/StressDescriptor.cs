using Core.Attributes;
using Game.Stress.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Stress.Descriptor
{
    [CreateAssetMenu(fileName = "StressDescriptor", menuName = "Dacha/Descriptors/StressDescriptor")]
    [Descriptor("Descriptors/" + nameof(StressDescriptor))]
    public class StressDescriptor : ScriptableObject
    {
        [field: SerializeField]
        public SerializedDictionary<StressType, StressModelDescriptor> Items { get; set; } = new();
    }
}