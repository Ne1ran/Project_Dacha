using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Diseases.Descriptor
{
    [CreateAssetMenu(fileName = "DiseasesDescriptor", menuName = "Dacha/Descriptors/DiseasesDescriptor")]
    [Descriptor("Descriptors/" + nameof(DiseasesDescriptor))]
    public class DiseasesDescriptor : Descriptor<string, DiseaseModelDescriptor>
    {
        [field: SerializeField]
        [TableList]
        public List<DiseaseModelDescriptor> Items { get; private set; } = new();
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<string, DiseaseModelDescriptor> dict = new();
            
            foreach (DiseaseModelDescriptor items in Items) {
                dict.Add(items.Id, items);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}