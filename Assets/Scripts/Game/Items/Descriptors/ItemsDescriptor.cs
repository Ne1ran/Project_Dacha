using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Items.Descriptors
{
    [CreateAssetMenu(fileName = "ItemsDescriptor", menuName = "Dacha/Descriptors/ItemsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ItemsDescriptor))]
    public class ItemsDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<ItemDescriptorModel> ItemDescriptors { get; private set; } = new();
    }
}