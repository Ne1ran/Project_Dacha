using System.Collections.Generic;
using Core.Attributes;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Fertilizers.Descriptor
{
    [CreateAssetMenu(fileName = "FertilizersDescriptor", menuName = "Dacha/Descriptors/FertilizersDescriptor")]
    [Descriptor("Descriptors/" + nameof(FertilizersDescriptor))]
    public class FertilizersDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<FertilizerDescriptorModel> Fertilizers { get; private set; } = new();
    }
}