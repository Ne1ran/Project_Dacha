using Core.Parameters;
using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.GameMap.Soil.Component;
using Game.GameMap.Tiles.Model;
using Game.Interactable.Model;
using Game.PieMenu.Service;
using Game.Plants.Component;
using UnityEngine;
using VContainer;

namespace Game.GameMap.Tiles.Component
{
    [PrefabPath("Prefabs/Tiles/pfTile")]
    public class TileController : MonoBehaviour, IInteractableComponent
    {
        [ComponentBinding("SoilHolder")]
        private Transform _soilHolder = null!;
        [ComponentBinding("PlantHolder")]
        private Transform _plantHolder = null!;
        
        [Inject]
        private readonly PieMenuService _pieMenuService = null!;

        public SingleTileModel TileModel { get; private set; } = null!;
        
        private SoilController? _currentSoil;
        private PlantController? _currentPlant;
        
        public void Initialize(SingleTileModel model)
        {
            TileModel = model;
        }

        public async UniTask Interact()
        {
            await _pieMenuService.CreatePieMenuAsync(InteractableType.TILE, new(ParameterNames.TileId, TileModel.Id));
        }

        public UniTask StopInteract()
        {
            return UniTask.CompletedTask;
        }

        public void AddSoil(SoilController soil)
        {
            _currentSoil = soil;
            soil.transform.SetParent(_soilHolder, false);
        }

        public void AddPlant(PlantController plant)
        {
            _currentPlant = plant;
            plant.transform.SetParent(_plantHolder, false);
        }
    }
}