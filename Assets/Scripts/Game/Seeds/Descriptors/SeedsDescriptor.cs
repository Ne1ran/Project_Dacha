using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Seeds.Descriptors
{
    [CreateAssetMenu(fileName = "SeedsDescriptor", menuName = "Dacha/Descriptors/SeedsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SeedsDescriptor))]
    public class SeedsDescriptor : Descriptor<string, SeedsDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<SeedsDescriptorModel> Items { get; private set; } = new();
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, SeedsDescriptorModel> dict = new();
            
            foreach (SeedsDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}