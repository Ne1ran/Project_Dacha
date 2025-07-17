using Core.Scene.Event;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

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
            _publisher.Publish(SceneChangedEvent.SCENE_PRELOAD, new(sceneAddress));

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneAddress, mode);
            SceneInstance result = await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                SceneInstance? oldScene = _loadedScene;
                _loadedScene = result;
                UnityEngine.SceneManagement.Scene scene = result.Scene;
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

            AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(sceneInstance);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                _publisher.Publish(SceneChangedEvent.SCENE_UNLOADED, new(sceneName));
            }
        }

        private void ActivateScene(UnityEngine.SceneManagement.Scene scene)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in rootGameObjects) {
                if (gameObject.TryGetComponent(out LifetimeScope lifetimeScope)) {
                    lifetimeScope.Build();
                    return;
                }
            }
        }
        
        public UnityEngine.SceneManagement.Scene? Scene => _loadedScene?.Scene;
    }
}