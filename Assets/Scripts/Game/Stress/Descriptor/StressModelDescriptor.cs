using System;
using System.Collections.Generic;
using Game.Plants.Model;
using Game.Symptoms.Descriptor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Stress.Descriptor
{
    [Serializable]
    public class StressModelDescriptor
    {
        [field: SerializeField, Range(0f, 1f), Tooltip("Threshold from max stress to start showing stress symptoms")]
        public float SymptomsShowThreshold { get; set; } = 0.25f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Chance of showing symptom on inspection")]
        public float SymptomShowChance { get; set; } = 0.5f;
        [field: SerializeField, Tooltip("List of plant family symptoms. They are unique to plant family")]
        public SerializedDictionary<PlantFamilyType, List<string>> PlantFamilySymptoms { get; set; } = new();
        [field: SerializeField, ValueDropdown("GetSymptoms", DrawDropdownForListElements = true),
                Tooltip("Common symptoms for all plants. If symptom is not available, it won't show but roll next possible symptom.")]
        public List<string> Symptoms { get; set; } = new();

        public List<string> GetSymptoms()
        {
            SymptomsDescriptor symptomsDescriptor = Resources.Load<SymptomsDescriptor>("Descriptors/SymptomsDescriptor");
            List<string> result = new();
            foreach ((string symptomName, SymptomDescriptorModel _) in symptomsDescriptor.Values) {
                result.Add(symptomName);
            }

            return result;
        }
    }
}