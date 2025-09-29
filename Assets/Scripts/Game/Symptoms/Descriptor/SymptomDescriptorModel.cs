using System;
using System.Collections.Generic;
using Game.Plants.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Symptoms.Descriptor
{
    [Serializable]
    public class SymptomDescriptorModel
    {
        [field: SerializeField, Tooltip("All descriptions for this symptom with tiers of knowledge.")]
        public SerializedDictionary<int, string> Descriptions { get; set; } = new();
        [field: SerializeField, Tooltip("Plant family types that can't have this symptom")]
        public List<PlantFamilyType> BannedFamilyTypes { get; set; } = new();
    }
}