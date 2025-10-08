using System;
using System.Collections.Generic;
using Game.Symptoms.Descriptor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class InfectionStage
    {
        [field: SerializeField, Tooltip("Infection stage of a plant")]
        public int Stage { get; set; } = 1;
        [field: SerializeField, Tooltip("Damage dealt to plant everyday if infection is on current stage"), Range(0f, 100f)]
        public float PlantDamagePerDay { get; set; } = 1f;
        [field: SerializeField, ValueDropdown("GetSymptoms", DrawDropdownForListElements = true), Tooltip("Random symptoms that player can see to identify disease")]
        public List<string> RandomSymptoms { get; set; } = new();
        [field: SerializeField, Tooltip("Total amount of growth points to change stage"), Range(0f, 100f)]
        public float StageGrowth { get; set; } = 25f;
        [field: SerializeField, Tooltip("Base speed of infection growth of current stage per day"), Range(0f, 100f)]
        public float BaseGrowthPerDay { get; set; } = 1f;
        [field: SerializeField, Tooltip("Base chance of infection heal per day on current stage"), Range(0f, 1f)]
        public float HealChance { get; set; } = 0.1f;

        public List<string> GetSymptoms()
        {
            SymptomsDescriptor symptomsDescriptor = Resources.Load<SymptomsDescriptor>("Descriptors/SymptomsDescriptor");
            List<string> result = new();
            foreach ((string symptomName, SymptomDescriptorModel _) in symptomsDescriptor.Items) {
                result.Add(symptomName);
            }

            return result;
        }
    }
}