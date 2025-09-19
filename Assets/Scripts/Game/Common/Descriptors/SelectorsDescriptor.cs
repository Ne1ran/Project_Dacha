using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Common.Descriptors
{
    [CreateAssetMenu(fileName = "SelectorsDescriptor", menuName = "Dacha/Descriptors/SelectorsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SelectorsDescriptor))]
    public class SelectorsDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<SelectorDescriptorModel> Items { get; private set; } = new();

        public SelectorDescriptorModel Require(string id)
        {
            SelectorDescriptorModel? selectorDescriptorModel = Items.Find(desc => desc.Id == id);
            if (selectorDescriptorModel == null) {
                throw new KeyNotFoundException($"Selector not found with id {id}!");
            }

            return selectorDescriptorModel;
        }
    }
}