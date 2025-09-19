using System.Collections.Generic;
using Core.Attributes;
using Game.Utils;
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

        public ItemDescriptorModel FindById(string id)
        {
            ItemDescriptorModel? itemDescriptorModel = ItemDescriptors.Find(item => item.ItemId == id);
            if (itemDescriptorModel == null) {
                throw new KeyNotFoundException($"Item was not found with id {id}!");
            }

            return itemDescriptorModel;
        }
        
        
        private void OnValidate()
        {
            foreach (ItemDescriptorModel? item in ItemDescriptors) {
                if (!string.IsNullOrEmpty(item.ItemId)) {
                    continue;
                }
                
                string newName = item.ItemName.Trim();
                string[] nameSplit = newName.Split(" ");

                string result = "";
                for (int i = 0; i < nameSplit.Length; i++) {
                    string part = nameSplit[i];
                    if (i > 0) {
                        result += part.ToLowerFirst();
                    } else {
                        result += part;
                    }
                }

                item.ItemId = result;
            }
        }
    }
}