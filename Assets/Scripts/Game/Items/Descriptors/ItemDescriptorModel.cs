using System;
using Game.Inventory.Model;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Items.Descriptors
{
    [Serializable]
    public class ItemDescriptorModel
    {
        [field: SerializeField]
        public string ItemId { get; set; } = string.Empty;
        [field: SerializeField]
        public string ItemName { get; set; } = string.Empty;
        [field: SerializeField, PreviouslySerializedAs("ItemPrefab")]
        public AssetReference WorldItemPrefab { get; set; } = null!;
        [field: SerializeField]
        public AssetReference HandsItemPrefab { get; set; } = null!;
        [field: SerializeField]
        public AssetReference? Icon { get; set; }
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