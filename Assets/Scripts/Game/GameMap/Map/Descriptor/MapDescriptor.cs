using System;
using Core.Attributes;
using Game.GameMap.Soil.Model;
using UnityEngine;

namespace Game.GameMap.Map.Descriptor
{
    [CreateAssetMenu(fileName = "MapDescriptor", menuName = "Dacha/Descriptors/MapDescriptor")]
    [Descriptor("Descriptors/" + nameof(MapDescriptor))]
    [Serializable]
    public class MapDescriptor : ScriptableObject
    {
        [field: SerializeField]
        public int Length { get; private set; } = 20;
        [field: SerializeField]
        public int Width { get; private set; } = 20;
        [field: SerializeField]
        public int TileLength { get; private set; } = 1;
        [field: SerializeField]
        public Vector3 TileMainPoint { get; private set; } = Vector3.zero;
        [field: SerializeField]
        public SoilType SoilType { get; private set; } = SoilType.Black;
    }
}