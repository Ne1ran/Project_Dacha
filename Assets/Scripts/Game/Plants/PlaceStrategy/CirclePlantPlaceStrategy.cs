using System.Collections.Generic;
using System.Linq;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine;

namespace Game.Plants.PlaceStrategy
{
    public class CirclePlantPlaceStrategy : IPlantPlaceStrategy
    {
        private IResourceService _resourceService = null!;

        private Transform _parent = null!;
        private List<Transform> _createdPlants = new();

        public void Initialize(IResourceService resourceService, Transform parent)
        {
            _resourceService = resourceService;
            _parent = parent;
        }

        public async UniTask PlaceAsync(string plantPrefab, int plantsCount, float offset, float tileLength)
        {
            List<UniTask<Transform>> list = new();

            List<Vector2> positions = new();
            if (plantsCount == 1) {
                positions.Add(Vector2.zero);
            } else {
                int plantsLength = Mathf.CeilToInt(Mathf.Sqrt(plantsCount));
                float range = tileLength / 2f - offset;
                float plantOffset = range * 2 / (plantsLength - 1);
                for (float x = -range; x <= range; x += plantOffset) {
                    for (float y = range; y >= -range; y -= plantOffset) {
                        positions.Add(new(x, y));
                    }
                }
            }

            foreach (Vector2 position in positions) {
                list.Add(CreatePlantPrefab(plantPrefab, position));
            }

            Transform[] plantsVisualizations = await UniTask.WhenAll(list);
            _createdPlants = plantsVisualizations.ToList();
        }

        private async UniTask<Transform> CreatePlantPrefab(string plantPrefab, Vector2 relativePosition)
        {
            Transform plantVisualPrefab = await _resourceService.LoadObjectAsync<Transform>(plantPrefab);
            plantVisualPrefab.localPosition = new(relativePosition.x, plantVisualPrefab.localPosition.y, relativePosition.y); ;
            plantVisualPrefab.SetParent(_parent, false);
            return plantVisualPrefab;
        }

        public void RemoveAll()
        {
            foreach (Transform createdPlant in _createdPlants) {
                createdPlant.DestroyObject();
            }
        }
    }
}