using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Stress.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Stress.Descriptor
{
    [CreateAssetMenu(fileName = "StressDescriptor", menuName = "Dacha/Descriptors/StressDescriptor")]
    [Descriptor("Descriptors/" + nameof(StressDescriptor))]
    public class StressDescriptor : Descriptor<StressType, StressModelDescriptor>
    {
        [field: SerializeField]
        public SerializedDictionary<StressType, StressModelDescriptor> Items { get; set; } = new();
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<StressType, StressModelDescriptor> dict = new();
            
            foreach (KeyValuePair<StressType, StressModelDescriptor> items in Items) {
                dict.Add(items.Key, items.Value);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}