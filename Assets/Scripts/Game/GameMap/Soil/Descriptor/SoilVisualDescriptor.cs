using System;
using Game.GameMap.Soil.Model;
using UnityEngine;

namespace Game.GameMap.Soil.Descriptor
{
    [Serializable]
    public class SoilVisualDescriptor
    {
        [field: SerializeField]
        public string BaseViewPrefab { get; set; } = "Prefabs/Soil/pfGrass";
        [field: SerializeField]
        public string BareSoilPrefab { get; set; } = "Prefabs/Soil/pfSoil";
        [field: SerializeField]
        public string TiltedPrefab { get; set; } = "Prefabs/Soil/pfSoilTilted";
        [field: SerializeField]
        public string TiltedAndWateredPrefab { get; set; } = "Prefabs/Soil/pfSoilTiltedWatered";

        public string GetPrefabPath(SoilState soilState, bool dugRecently, bool wellWatered)
        {
            return soilState switch {
                    SoilState.None => dugRecently ? BareSoilPrefab : BaseViewPrefab,
                    SoilState.Tilted => wellWatered ? TiltedAndWateredPrefab : TiltedPrefab,
                    SoilState.Planted => wellWatered ? TiltedAndWateredPrefab : TiltedPrefab,
                    _ => throw new ArgumentException($"Soil state not found={soilState.ToString()}")
            };
        }
    }
}