using System;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class TileRangePair
    {
        [field: SerializeField]
        public int TileRange { get; set; } = 1;
        [field: SerializeField, Range(0f, 10f)]
        public float Multiplier { get; set; } = 1;
    }
}