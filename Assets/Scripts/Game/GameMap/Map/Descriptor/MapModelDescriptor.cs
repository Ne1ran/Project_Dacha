using System;
using Game.Soil.Model;
using UnityEngine;

namespace Game.GameMap.Map.Descriptor
{
    [Serializable]
    public class MapModelDescriptor
    {
        [field: SerializeField]
        public int Length { get; set; } = 20;
        [field: SerializeField]
        public int Width { get; set; } = 20;
        [field: SerializeField]
        public int TileLength { get; set; } = 1;
        [field: SerializeField]
        public Vector3 TileMainPoint { get; set; } = Vector3.zero;
        [field: SerializeField]
        public SoilType SoilType { get; set; } = SoilType.Black;
    }
}