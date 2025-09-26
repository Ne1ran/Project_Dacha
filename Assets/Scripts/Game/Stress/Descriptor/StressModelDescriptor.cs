using System;
using UnityEngine;

namespace Game.Stress.Descriptor
{
    [Serializable]
    public class StressModelDescriptor
    {
        [field: SerializeField, Range(0f, 1f), Tooltip("Threshold from max stress to start showing stress symptoms")]
        public float SymptomsShowThreshold { get; set; } = 0.35f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Chance of showing symptom on inspection")]
        public float SymptomShowChance { get; set; } = 0.5f;
    }
}