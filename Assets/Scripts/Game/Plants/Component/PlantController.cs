using System;
using Core.Descriptors.Service;
using Core.Parameters;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.GameMap.Map.Descriptor;
using Game.Interactable.Model;
using Game.PieMenu.Service;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Plants.PlaceStrategy;
using UnityEngine;
using VContainer;

namespace Game.Plants.Component
{
    [NeedBinding("pfPlantController")]
    public class PlantController : MonoBehaviour, IInteractableComponent
    {
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
        [Inject]
        private readonly IResourceService _resourceService = null!;
        [Inject]
        private readonly PieMenuService _pieMenuService = null!;

        private IPlantPlaceStrategy? _plantPlaceStrategy;

        private PlantModel _plantModel = null!;
        private PlantGrowStage _currentStage;
        private string _tileId = null!;

        public async UniTask InitializeAsync(string tileId, PlantModel plantModel)
        {
            _tileId = tileId;
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
                visualPrefabPath = plantsDescriptorModel.Visualization.DeadPrefab.AssetGUID;
            } else {
                PlantStageDescriptor plantStageDescriptor = plantsDescriptorModel.RequireStage(plantModel.CurrentStage);
                visualPrefabPath = plantStageDescriptor.Prefab.AssetGUID;
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

        public async UniTask Interact()
        {
            await _pieMenuService.CreatePieMenuAsync(InteractableType.PLANT, new(ParameterNames.TileId, _tileId));
        }

        public UniTask StopInteract()
        {
            return UniTask.CompletedTask;
        }
    }
}