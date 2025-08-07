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
        public ItemType ItemType { get; set; }
        [field: SerializeField]
        public bool Stackable { get; set; } = false;
        [field: SerializeField]
        public bool ShowInHand { get; set; } = false;
        [field: SerializeField, ShowIf("Stackable")]
        public int MaxStack { get; set; } = 1;
    }
}