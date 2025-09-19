using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [CreateAssetMenu(fileName = "PlantsDescriptor", menuName = "Dacha/Descriptors/PlantsDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantsDescriptor))]
    public class PlantsDescriptor : ScriptableObject
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
    }
}