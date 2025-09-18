using Core.Attributes;
using Core.SceneManagement.Event;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Core.SceneManagement.Service
{
    [Service]
    public class SceneService
    {
        private SceneInstance? _loadedScene;
        
        private readonly IPublisher<string, SceneChangedEvent> _publisher;

        public SceneService(IPublisher<string, SceneChangedEvent> publisher)
        {
            _publisher = publisher;
        }

        public async UniTask LoadSceneAsync(string sceneAddress, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _publisher.Publish(SceneChangedEvent.SCENE_PRELOAD, new(sceneAddress));

            AsyncOperationHandle<SceneInstance> handle = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(sceneAddress, mode);
            SceneInstance result = await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                SceneInstance? oldScene = _loadedScene;
                _loadedScene = result;
                Scene scene = result.Scene;
                _publisher.Publish(SceneChangedEvent.SCENE_LOADED, new(scene.name, scene));
                ActivateScene(scene);
                await UnloadSceneAsync(oldScene);
            }
        }

        public async UniTask UnloadSceneAsync(SceneInstance? oldScene)
        {
            if (oldScene == null) {
                return;
            }

            SceneInstance sceneInstance = oldScene.Value;

            if (!sceneInstance.Scene.isLoaded) {
                return;
            }

            string sceneName = sceneInstance.Scene.name;
            _publisher.Publish(SceneChangedEvent.SCENE_PREUNLOAD, new(sceneName));

            AsyncOperationHandle<SceneInstance> handle = UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(sceneInstance);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                _publisher.Publish(SceneChangedEvent.SCENE_UNLOADED, new(sceneName));
            }
        }

        private void ActivateScene(Scene scene)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in rootGameObjects) {
                if (gameObject.TryGetComponent(out LifetimeScope lifetimeScope)) {
                    lifetimeScope.Build();
                    return;
                }
            }
        }
        
        public Scene? Scene => _loadedScene?.Scene;
    }
}