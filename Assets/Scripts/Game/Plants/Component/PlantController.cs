using System;
using Core.Descriptors.Service;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Map.Descriptor;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Plants.PlaceStrategy;
using UnityEngine;
using VContainer;

namespace Game.Plants.Component
{
    [PrefabPath("Prefabs/Plants/pfPlantController")]
    public class PlantController : MonoBehaviour
    {
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
        [Inject]
        private readonly IResourceService _resourceService = null!;

        private IPlantPlaceStrategy? _plantPlaceStrategy;

        private PlantModel _plantModel = null!;
        private PlantGrowStage _currentStage;

        public async UniTask InitializeAsync(PlantModel plantModel)
        {
            _plantModel = plantModel;
            _currentStage = _plantModel.CurrentStage;
            await PlaceAllPlantsAsync(_plantModel);
        }

        private async UniTask PlaceAllPlantsAsync(PlantModel plantModel)
        {
            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            PlantsDescriptorModel plantsDescriptorModel = plantsDescriptor.RequirePlant(plantModel.PlantId);
            string visualPrefabPath;
            if (plantModel.CurrentStage == PlantGrowStage.DEAD) {
                visualPrefabPath = plantsDescriptorModel.Visualization.DeadPrefabPath;
            } else {
                PlantStageDescriptor plantStageDescriptor = plantsDescriptorModel.RequireStage(plantModel.CurrentStage);
                visualPrefabPath = plantStageDescriptor.PrefabPath;
            }
            
            if (string.IsNullOrEmpty(visualPrefabPath)) {
                return;
            }

            MapDescriptor mapDescriptor = _descriptorService.Require<MapDescriptor>();
            int tileLength = mapDescriptor.TileLength;
            int plantsCount = plantsDescriptorModel.PlantsCount;
            PlantVisualizationDescriptor plantVisualizationDescriptor = plantsDescriptorModel.Visualization;
            if (_plantPlaceStrategy == null) {
                _plantPlaceStrategy = SelectPlaceStrategy(plantVisualizationDescriptor.Type);
                _plantPlaceStrategy.Initialize(_resourceService, transform);
            }

            await _plantPlaceStrategy.PlaceAsync(visualPrefabPath, plantsCount, plantVisualizationDescriptor.Offset, tileLength);
        }

        public void UpdatePlantsIfNeeded(PlantModel newPlantModel)
        {
            if (_plantModel.CurrentStage == _currentStage) {
                return;
            }

            _currentStage = _plantModel.CurrentStage;
            
            if (_plantPlaceStrategy == null) {
                Debug.LogWarning("Plant place strategy is null. This should not be possible!");
                return;
            }

            _plantPlaceStrategy.RemoveAll();
            PlaceAllPlantsAsync(_plantModel).Forget();
        }

        private IPlantPlaceStrategy SelectPlaceStrategy(PlantVisualizationType visualizationType)
        {
            return visualizationType switch {
                    PlantVisualizationType.Circle => new CirclePlantPlaceStrategy(),
                    _ => throw new NotImplementedException($"Unknown plantVisualization type={visualizationType.ToString()}")
            };
        }
    }
}