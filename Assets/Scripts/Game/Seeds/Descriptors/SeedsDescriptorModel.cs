using System;
using Game.Seeds.Model;
using UnityEngine;

namespace Game.Seeds.Descriptors
{
    [Serializable]
    public class SeedsDescriptorModel
    {
        [field: SerializeField]
        public string SeedId { get; set; } = null!;
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
    }
}