using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Harvest.Descriptor
{
    [CreateAssetMenu(fileName = "PlantHarvestDescriptor", menuName = "Dacha/Descriptors/PlantHarvestDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantHarvestDescriptor))]
    public class PlantHarvestDescriptor : Descriptor<string, PlantHarvestDescriptorModel>
    {
        [field: SerializeField, TableList]
        public List<PlantHarvestDescriptorModel> Items { get; set; } = new();

        public PlantHarvestDescriptorModel? FindItemById(string id)
        {
            return Items.Find(hdm => hdm.Id == id);
        }
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, PlantHarvestDescriptorModel> dict = new();
            
            foreach (PlantHarvestDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}