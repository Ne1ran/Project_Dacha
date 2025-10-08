using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Tools.Descriptors
{
    [CreateAssetMenu(fileName = "ToolsDescriptor", menuName = "Dacha/Descriptors/ToolsDescriptor")]
    [Descriptor("Descriptors/" + nameof(ToolsDescriptor))]
    public class ToolsDescriptor : Descriptor<string, ToolsDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<ToolsDescriptorModel> Items { get; private set; } = new();
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, ToolsDescriptorModel> dict = new();
            
            foreach (ToolsDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
