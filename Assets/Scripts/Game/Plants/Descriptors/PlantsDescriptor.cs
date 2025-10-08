using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Plants.Descriptors
{
    [CreateAssetMenu(fileName = "PlantsDescriptor", menuName = "Dacha/Descriptors/PlantsDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantsDescriptor))]
    public class PlantsDescriptor : Descriptor<string, PlantsDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<PlantsDescriptorModel> Items { get; private set; } = new();

        public PlantsDescriptorModel RequirePlant(string id)
        {
            PlantsDescriptorModel? plantsDescriptorModel = Items.Find(plant => plant.Id == id);
            if (plantsDescriptorModel == null) {
                throw new KeyNotFoundException($"Plant not found with id={id}");
            }
            
            return plantsDescriptorModel;
        }
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, PlantsDescriptorModel> dict = new();
            
            foreach (PlantsDescriptorModel items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}