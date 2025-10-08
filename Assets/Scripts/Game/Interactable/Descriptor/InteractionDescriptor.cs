using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Interactable.Model;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Interactable.Descriptor
{
    [CreateAssetMenu(fileName = "InteractionDescriptor", menuName = "Dacha/Descriptors/InteractionDescriptor")]
    [Descriptor("Descriptors/" + nameof(InteractionDescriptor))]
    public class InteractionDescriptor : Descriptor<InteractableType, InteractionDescriptorModel>
    {
        [field: SerializeField]
        [TableList]
        public List<InteractionDescriptorModel> Items { get; private set; } = new();

        public InteractionDescriptorModel RequireByType(InteractableType soilType)
        {
            InteractionDescriptorModel? descriptorModel = Items.Find(desc => desc.InteractableType == soilType);
            if (descriptorModel == null) {
                throw new KeyNotFoundException($"Interaction descriptor not found with type={soilType}");
            }

            return descriptorModel;
        }
        
        
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<InteractableType, InteractionDescriptorModel> dict = new();
            
            foreach (InteractionDescriptorModel items in Items) {
                dict.Add(items.InteractableType, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}