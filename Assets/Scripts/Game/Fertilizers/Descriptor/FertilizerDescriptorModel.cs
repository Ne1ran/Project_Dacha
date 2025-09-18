using System;
using Game.Fertilizers.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Fertilizers.Descriptor
{
    [Serializable]
    public class FertilizerDescriptorModel
    {
        [field: SerializeField]
        public string Id { get; set; } = string.Empty;
        [field: SerializeField]
        public string Name { get; set; } = string.Empty;
        [field: SerializeField]
        public FertilizerType Type { get; set; } = FertilizerType.Mineral;
        [field: SerializeField, Range(100f, 5000f), Tooltip("Starting fertilizer mass")]
        public float StartMass { get; set; } = 1000f;
        [field: SerializeField, Range(1, 1000), Tooltip("Time in days for fertilizer to decompose and give all of its elements to the soil")]
        public int DecomposeTime { get; set; } = 14;
        [field: SerializeField, Range(-6f, 6f), Tooltip("PH change fertilizer can create if used with !full mass!")]
        public float PhChange { get; set; } = 0f;
        [field: SerializeField, Range(-1000f, 1000f), Tooltip("Value that influence the soil humus amount in gramms")]
        public float HumusValue { get; set; } = 0f;
        [field: SerializeField, Range(-25f, 25f), Tooltip("Value that influence the soil breathability change after full decomposition")]
        public float BreathabilityValue { get; set; } = 0f;
        [field: SerializeField]
        public AssetReference Prefab { get; set; } = null!;
        [field: SerializeField]
        public FertilizerElementsDescriptorModel FertilizerElements { get; set; } = null!;
    }
}