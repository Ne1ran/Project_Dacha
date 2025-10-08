using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Soil.Model;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Soil.Descriptor
{
    [CreateAssetMenu(fileName = "SoilDescriptor", menuName = "Dacha/Descriptors/SoilDescriptor")]
    [Descriptor("Descriptors/" + nameof(SoilDescriptor))]
    public class SoilDescriptor : Descriptor<SoilType, SoilDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<SoilDescriptorModel> Items { get; private set; } = new();

        public SoilDescriptorModel RequireByType(SoilType soilType)
        {
            SoilDescriptorModel? soilDescriptorModel = Items.Find(desc => desc.SoilType == soilType);
            if (soilDescriptorModel == null) {
                throw new KeyNotFoundException($"Soil not found with type={soilType}");
            }

            return soilDescriptorModel;
        }
        
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<SoilType, SoilDescriptorModel> dict = new();
            
            foreach (SoilDescriptorModel items in Items) {
                dict.Add(items.SoilType, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}