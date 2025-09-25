using System.Collections.Generic;
using Core.Attributes;
using Game.Difficulty.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Temperature.Descriptor
{
    
    [CreateAssetMenu(fileName = "TemperatureDistributionDescriptor", menuName = "Dacha/Descriptors/TemperatureDistributionDescriptor")]
    [Descriptor("Descriptors/" + nameof(TemperatureDistributionDescriptor))]
    public class TemperatureDistributionDescriptor : ScriptableObject
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
    }
}