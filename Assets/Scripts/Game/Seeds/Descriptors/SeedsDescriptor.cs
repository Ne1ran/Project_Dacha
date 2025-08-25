using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Seeds.Descriptors
{
    [CreateAssetMenu(fileName = "SeedsDescriptor", menuName = "Dacha/Descriptors/SeedsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SeedsDescriptor))]
    public class SeedsDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<SeedsDescriptorModel> Items { get; private set; } = new();
    }
}