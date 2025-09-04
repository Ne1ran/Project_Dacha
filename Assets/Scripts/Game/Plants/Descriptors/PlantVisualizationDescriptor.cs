using System;
using Game.Plants.Model;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantVisualizationDescriptor
    {
        [field: SerializeField]
        public PlantVisualizationType Type { get; set; }
        [field: SerializeField]
        public float Offset { get; set; }
    }
}