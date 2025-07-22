using Core.SceneManagement.Service;
using Cysharp.Threading.Tasks;
using Game.Player.Controller;
using Game.Player.Service;
using Game.PlayMode.Service;
using Game.Spawn;
using Game.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.EntryPoints
{
    public class DachaEntryPoint : MonoBehaviour
    {
        [Inject]
        private PlayerService _playerService;
        [Inject]
        private SceneService _sceneService;
        [Inject]
        private PlayModeService _playModeService;
        
        private void Start()
        {
            Debug.Log("Starting Dacha");

            StartGameAsync().Forget();
        }

        private async UniTask StartGameAsync()
        {
            PlayerController playerController = await _playerService.CreatePlayerAsync();

            Scene scene = _sceneService.Scene!.Value;
            PlayerSpawnPoint playerSpawnPoint = scene.GetSceneRootObjectByType<PlayerSpawnPoint>();
            playerController.SetPosition(playerSpawnPoint);

            playerController.Initialize();

            await _playModeService.CreatePlayModeScreen();
        }
    }
}