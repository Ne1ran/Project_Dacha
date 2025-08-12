using Core.Descriptors.Service;
using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
using Game.Interactable.Model;
using Game.PieMenu.Service;
using UnityEngine;
using VContainer;

namespace Game.GameMap.Tiles.Component
{
    [PrefabPath("Prefabs/Tiles/pfTile")]
    public class TileController : MonoBehaviour, IInteractableComponent
    {
        [Inject]
        private readonly TileService _tileService = null!;
        [Inject]
        private readonly PieMenuService _pieMenuService = null!;
        [Inject]
        private readonly SoilService _soilService = null!;
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;

        private SingleTileModel _tileModel = null!;
        
        public void Initialize(SingleTileModel model)
        {
            _tileModel = model;
        }

        public async UniTask Interact()
        {
            if (_tileModel.Soil == null) {
                SoilType mapSoilType = _descriptorService.Require<MapDescriptor>().SoilType;
                SoilModel soilModel = _soilService.CreateSoil(mapSoilType);
                _tileService.UpdateSoil(_tileModel.Id, soilModel);
                Debug.Log("Added soil to tile!");
            }
            
            await _pieMenuService.CreatePieMenuAsync(InteractableType.TILE);
        }

        public UniTask StopInteract()
        {
            Debug.Log("Stopped interacting with tile!");
            return UniTask.CompletedTask;
        }
    }
}