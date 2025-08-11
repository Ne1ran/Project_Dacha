using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Core.UI.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
using Game.Interactable.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

namespace Game.GameMap.Tiles.Component
{
    [PrefabPath("Prefabs/Tiles/pfTile")]
    public class TileController : MonoBehaviour, IInteractableComponent
    {
        [Inject]
        private TileService _tileService = null!;
        [Inject]
        private UIService _uiService = null!;

        public void Initialize(SingleTileModel model)
        {
        }

        public async UniTask Interact()
        {
            await _uiService.ShowDialogAsync<PieMenuController>();
        }
    }
}