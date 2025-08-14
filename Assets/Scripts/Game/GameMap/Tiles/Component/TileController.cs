using Core.Descriptors.Service;
using Core.Parameters;
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
            await _pieMenuService.CreatePieMenuAsync(InteractableType.TILE, new(ParameterNames.TileId, _tileModel.Id));
        }

        public UniTask StopInteract()
        {
            return UniTask.CompletedTask;
        }
    }
}