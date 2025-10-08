using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Symptoms.Descriptor
{
    [CreateAssetMenu(fileName = "SymptomsDescriptor", menuName = "Dacha/Descriptors/SymptomsDescriptor")]
    [Descriptor("Descriptors/" + nameof(SymptomsDescriptor))]
    public class SymptomsDescriptor : Descriptor<string, SymptomDescriptorModel>
    {
        [field: SerializeField]
        public SerializedDictionary<string, SymptomDescriptorModel> Items { get; set; } = new();
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, SymptomDescriptorModel> dict = new();
            
            foreach (KeyValuePair<string, SymptomDescriptorModel> items in Items) {
                dict.Add(items.Key, items.Value);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}