using System.Collections.Generic;
using Core.Attributes;
using Game.Soil.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Soil.Descriptor
{
    [CreateAssetMenu(fileName = "SoilDescriptor", menuName = "Dacha/Descriptors/SoilDescriptor")]
    [Descriptor("Descriptors/" + nameof(SoilDescriptor))]
    public class SoilDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<SoilDescriptorModel> ItemDescriptors { get; private set; } = new();

        public SoilDescriptorModel RequireByType(SoilType soilType)
        {
            SoilDescriptorModel? soilDescriptorModel = ItemDescriptors.Find(desc => desc.SoilType == soilType);
            if (soilDescriptorModel == null) {
                throw new KeyNotFoundException($"Soil not found with type={soilType}");
            }

            return soilDescriptorModel;
        }
    }
}