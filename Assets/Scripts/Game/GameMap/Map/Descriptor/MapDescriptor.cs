using System;
using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Difficulty.Model;
using Game.Soil.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.GameMap.Map.Descriptor
{
    [CreateAssetMenu(fileName = "MapDescriptor", menuName = "Dacha/Descriptors/MapDescriptor")]
    [Descriptor("Descriptors/" + nameof(MapDescriptor))]
    [Serializable]
    public class MapDescriptor : Descriptor<DachaPlaceType, MapModelDescriptor>
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
        
        public void OnValidate()
        {
            SerializedDictionary<DachaPlaceType, MapModelDescriptor> dict = new();
            
            dict.Add(DachaPlaceType.Middle, new() {
                    Length = Length,
                    Width = Width,
                    TileLength = TileLength,
                    TileMainPoint = TileMainPoint,
                    SoilType = SoilType,
            });
            
            SetValues(dict);
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}