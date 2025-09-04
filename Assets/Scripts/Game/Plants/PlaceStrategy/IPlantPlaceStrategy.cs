using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Plants.PlaceStrategy
{
    public interface IPlantPlaceStrategy
    {
        void Initialize(IResourceService resourceService, Transform parent);
        UniTask PlaceAsync(string plantPrefab, int plantsCount, float offset, float tileLength);
        void RemoveAll();
    }
}