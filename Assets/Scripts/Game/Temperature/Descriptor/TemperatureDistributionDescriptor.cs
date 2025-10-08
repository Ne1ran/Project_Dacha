using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Difficulty.Model;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Temperature.Descriptor
{
    
    [CreateAssetMenu(fileName = "TemperatureDistributionDescriptor", menuName = "Dacha/Descriptors/TemperatureDistributionDescriptor")]
    [Descriptor("Descriptors/" + nameof(TemperatureDistributionDescriptor))]
    public class TemperatureDistributionDescriptor : Descriptor<DachaPlaceType, SerializedDictionary<int, float>>
    {
        [TableList]
        [field: SerializeField]
        public List<TemperatureDistributionModelDescriptor> Items { get; set; } = new();

        public TemperatureDistributionModelDescriptor FindByPlaceType(DachaPlaceType placeType)
        {
            TemperatureDistributionModelDescriptor? distributionModelDescriptor = Items.Find(x => x.PlaceType == placeType);
            if (distributionModelDescriptor == null) {
                throw new KeyNotFoundException($"There is no temperature distribution definition for {placeType.ToString()}");
            }

            return distributionModelDescriptor;
        }
        
        public void OnValidate()
        {
            if (Items.Count == 0) {
                return;
            }
            SerializedDictionary<DachaPlaceType, SerializedDictionary<int, float>> dict = new();
            
            foreach (TemperatureDistributionModelDescriptor items in Items) {
                SerializedDictionary<int, float> dict2 = new();
                foreach (KeyValuePair<int,float> keyValuePair in items.TemperatureDistribution) {
                    dict2.Add(keyValuePair.Key, keyValuePair.Value);
                }
                dict.Add(items.PlaceType, dict2);
            }
            
            SetValues(dict);
            
            Items.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}