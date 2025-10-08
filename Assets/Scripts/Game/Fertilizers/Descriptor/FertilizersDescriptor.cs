using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Fertilizers.Descriptor
{
    [CreateAssetMenu(fileName = "FertilizersDescriptor", menuName = "Dacha/Descriptors/FertilizersDescriptor")]
    [Descriptor("Descriptors/" + nameof(FertilizersDescriptor))]
    public class FertilizersDescriptor : Descriptor<string, FertilizerDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<FertilizerDescriptorModel> Items { get; private set; } = new();
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, FertilizerDescriptorModel> dict = new();
            
            foreach (FertilizerDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}