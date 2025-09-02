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
        public List<SelectorDescriptorModel> SelectorsDescriptors { get; private set; } = new();

        public SelectorDescriptorModel Require(string id)
        {
            SelectorDescriptorModel? selectorDescriptorModel = SelectorsDescriptors.Find(desc => desc.SelectorId == id);
            if (selectorDescriptorModel == null) {
                throw new KeyNotFoundException($"Selector not found with id {id}!");
            }

            return selectorDescriptorModel;
        }
    }
}