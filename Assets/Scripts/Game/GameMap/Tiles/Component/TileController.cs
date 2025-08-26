using Core.Parameters;
using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.GameMap.Tiles.Model;
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
        private readonly PieMenuService _pieMenuService = null!;

        public SingleTileModel TileModel { get; private set; } = null!;
        
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
    }
}