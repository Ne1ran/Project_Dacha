using Core.Scene.Event;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.Scene.Service
{
    [UsedImplicitly]
    public class SceneService
    {
        private SceneInstance? _loadedScene;

        [Inject]
        private IPublisher<string, SceneChangedEvent> _publisher;

        public async UniTask LoadSceneAsync(string sceneAddress, LoadSceneMode mode = LoadSceneMode.Single)
        {
            await UnloadSceneAsync();
            
            _publisher.Publish(SceneChangedEvent.SCENE_PRELOAD, new(sceneAddress));

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneAddress, mode);
            SceneInstance result = await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                _loadedScene = result;
                _publisher.Publish(SceneChangedEvent.SCENE_LOADED, new(result.Scene.name));
            }
        }

        public async UniTask UnloadSceneAsync()
        {
            if (!_loadedScene.HasValue) {
                return;
            }

            SceneInstance sceneInstance = _loadedScene.Value;

            if (!sceneInstance.Scene.isLoaded) {
                return;
            }

            string sceneName = sceneInstance.Scene.name;
            _publisher.Publish(SceneChangedEvent.SCENE_PREUNLOAD, new(sceneName));

            AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(sceneInstance);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                _publisher.Publish(SceneChangedEvent.SCENE_UNLOADED, new(sceneName));
            }
        }
    }
}