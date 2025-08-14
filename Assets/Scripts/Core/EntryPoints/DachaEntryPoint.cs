using Core.GameWorld.Service;
using Core.SceneManagement.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Map.Service;
using Game.GameMap.Spawn;
using Game.Interactable.Handlers;
using Game.Player.Controller;
using Game.Player.Service;
using Game.PlayMode.Service;
using Game.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.EntryPoints
{
    public class DachaEntryPoint : MonoBehaviour
    {
        [Inject]
        private PlayerService _playerService = null!;
        [Inject]
        private SceneService _sceneService = null!;
        [Inject]
        private PlayModeService _playModeService = null!;
        [Inject]
        private MapService _mapService = null!;
        [Inject]
        private GameWorldService _gameWorldService = null!;

        private void Start()
        {
            Debug.Log("Starting Dacha");
            StartGameAsync().Forget();
        }

        private async UniTask StartGameAsync()
        {
            _gameWorldService.Initialize();
            await _mapService.InitializeMapAsync();

            PlayerController playerController = await _playerService.CreatePlayerAsync();

            Scene scene = _sceneService.Scene!.Value;
            PlayerSpawnPoint playerSpawnPoint = scene.GetSceneRootObjectByType<PlayerSpawnPoint>();
            playerController.SetPosition(playerSpawnPoint);

            playerController.Initialize();

            await _playModeService.CreatePlayModeScreen();
        }
    }
}