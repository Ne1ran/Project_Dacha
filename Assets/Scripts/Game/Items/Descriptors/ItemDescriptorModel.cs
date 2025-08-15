using System;
using Game.Inventory.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Items.Descriptors
{
    [Serializable]
    public class ItemDescriptorModel
    {
        [field: SerializeField]
        public string ItemId { get; set; } = string.Empty;
        [field: SerializeField]
        public string? ItemPrefab { get; set; }
        [field: SerializeField]
        public Sprite Icon { get; set; } = null!; // todo neiran redo when integrate addressables properly
        [field: SerializeField]
        public ItemType ItemType { get; set; }
        [field: SerializeField]
        public float DropOffsetMultiplier { get; set; } = 2.5f;
        [field: SerializeField]
        public bool Stackable { get; set; } = false;
        [field: SerializeField]
        public bool ShowInHand { get; set; } = false;
        [field: SerializeField, ShowIf("Stackable")]
        public int MaxStack { get; set; } = 1;
    }
}