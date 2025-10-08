using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Common.Descriptors
{
    [CreateAssetMenu(fileName = "SelectorsDescriptor", menuName = "Dacha/Descriptors/SelectorsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SelectorsDescriptor))]
    public class SelectorsDescriptor : Descriptor<string, SelectorDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<SelectorDescriptorModel> Items { get; private set; } = new();
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            
            SerializedDictionary<string, SelectorDescriptorModel> dict = new();
            
            foreach (SelectorDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}