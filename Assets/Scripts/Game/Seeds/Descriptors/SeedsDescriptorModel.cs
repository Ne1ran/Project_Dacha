using System;
using System.Collections.Generic;
using Game.Plants.Descriptors;
using Game.Seeds.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Seeds.Descriptors
{
    [Serializable]
    public class SeedsDescriptorModel
    {
        [field: SerializeField]
        public string SeedId { get; set; } = null!;
        [field: SerializeField, ValueDropdown("GetPlantIds")]
        public string PlantId { get; set; } = null!;
        [field: SerializeField]
        public SeedType SeedType { get; set; } = SeedType.NONE;
        [field: SerializeField]
        public string SeedName { get; set; } = null!;
        [field: SerializeField]
        public string SeedPrefab { get; set; } = null!;
        [field: SerializeField]
        public string UseHandler { get; set; } = null!;
        [field: SerializeField]
        public bool CanBeInfected { get; set; }
        [field: SerializeField, Tooltip("Start health of a plant if used seed")]
        public float StartHealth { get; set; } = 100f;
        [field: SerializeField, Tooltip("Start immunity of a plant if used seed")]
        public float StartImmunity { get; set; } = 10f;

        public List<string> GetPlantIds()
        {
            PlantsDescriptor plantsDescriptor = Resources.Load<PlantsDescriptor>("Descriptors/PlantsDescriptor");
            List<string> result = new();
            foreach (PlantsDescriptorModel pdm in plantsDescriptor.Items) {
                result.Add(pdm.PlantId);
            }

            return result;
        }
    }
}