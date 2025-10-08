using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Items.Descriptors
{
    [CreateAssetMenu(fileName = "ItemsDescriptor", menuName = "Dacha/Descriptors/ItemsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ItemsDescriptor))]
    public class ItemsDescriptor : Descriptor<string, ItemDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<ItemDescriptorModel> Items { get; private set; } = new();

        public ItemDescriptorModel FindById(string id)
        {
            ItemDescriptorModel? itemDescriptorModel = Items.Find(item => item.Id == id);
            if (itemDescriptorModel == null) {
                throw new KeyNotFoundException($"Item was not found with id {id}!");
            }

            return itemDescriptorModel;
        }
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, ItemDescriptorModel> dict = new();
            
            foreach (ItemDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}