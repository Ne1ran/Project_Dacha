using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
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
        private TileService _tileService = null!;
        [Inject]
        private PieMenuService _pieMenuService = null!;

        public void Initialize(SingleTileModel model)
        {
        }

        public async UniTask Interact()
        {
            await _pieMenuService.CreatePieMenuAsync(InteractableType.TILE);
        }
    }
}