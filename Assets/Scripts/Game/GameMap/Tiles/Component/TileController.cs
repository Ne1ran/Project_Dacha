using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.GameMap.Tiles.Model;
using Game.GameMap.Tiles.Service;
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
        
        public void Initialize(SingleTileModel model)
        {
            
        }

        public UniTask Interact()
        {
            return UniTask.CompletedTask;
        }
    }
}