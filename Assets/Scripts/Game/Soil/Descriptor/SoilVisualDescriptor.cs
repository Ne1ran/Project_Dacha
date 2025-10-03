using System;
using Game.Soil.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Soil.Descriptor
{
    [Serializable]
    public class SoilVisualDescriptor
    {
        [field: SerializeField]
        public AssetReference BaseViewPrefab { get; set; } = null!;
        [field: SerializeField]
        public AssetReference BareSoilPrefab { get; set; } = null!;
        [field: SerializeField]
        public AssetReference TiltedPrefab { get; set; } = null!;
        [field: SerializeField]
        public AssetReference TiltedAndWateredPrefab { get; set; } = null!;

        public string GetPrefabPath(SoilState soilState, bool dugRecently, bool wellWatered)
        {
            AssetReference assetReference = soilState switch {
                    SoilState.None => dugRecently ? BareSoilPrefab : BaseViewPrefab,
                    SoilState.Tilted => wellWatered ? TiltedAndWateredPrefab : TiltedPrefab,
                    SoilState.Planted => wellWatered ? TiltedAndWateredPrefab : TiltedPrefab,
                    _ => throw new ArgumentException($"Soil state not found={soilState.ToString()}")
            };

            return assetReference.AssetGUID;
        }
    }
}