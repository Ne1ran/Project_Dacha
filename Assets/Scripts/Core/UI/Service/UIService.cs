using System;
using System.Collections.Generic;
using Core.Resources.Service;
using Core.SceneManagement.Event;
using Core.SceneManagement.Service;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Core.UI.Service
{
    public class UIService : IInitializable, IDisposable
    {
        private readonly ISubscriber<string, SceneChangedEvent> _subscriber;
        private readonly IResourceService _resourceService;
        private readonly SceneService _sceneService;

        private Canvas? _currentCanvas;

        //todo neiran add priority with dialogs

        private readonly Dictionary<Type, GameObject> _dialogs = new();
        private readonly List<GameObject> _elements = new();

        private IDisposable? _disposable;

        public UIService(ISubscriber<string, SceneChangedEvent> subscriber,
                         IResourceService resourceService,
                         SceneService sceneService)
        {
            _subscriber = subscriber;
            _resourceService = resourceService;
            _sceneService = sceneService;
        }

        public void Initialize()
        {
            DisposableBagBuilder bagBuilder = DisposableBag.CreateBuilder();
            // bagBuilder.Add(_subscriber.Subscribe(SceneChangedEvent.SCENE_PRELOAD, OnScenePreloaded));
            // bagBuilder.Add(_subscriber.Subscribe(SceneChangedEvent.SCENE_UNLOADED, OnSceneUnloaded));
            bagBuilder.Add(_subscriber.Subscribe(SceneChangedEvent.SCENE_LOADED, OnSceneChanged));
            _disposable = bagBuilder.Build();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        private void OnSceneChanged(SceneChangedEvent evt)
        {
            if (!evt.Scene.HasValue) {
                Debug.LogWarning("Scene changed, but no scene was found!");
                return;
            }

            UnityEngine.SceneManagement.Scene scene = evt.Scene.Value;
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject rgo in rootGameObjects) {
                if (!rgo.TryGetComponent(out Canvas canvas)) {
                    continue;
                }

                _currentCanvas = canvas;
                return;
            }

            Debug.LogWarning($"Canvas not found on scene. SceneName = {evt.SceneName}");
        }

        public async UniTask<T> ShowDialogAsync<T>()
                where T : Component
        {
            if (_currentCanvas == null) {
                throw new NullReferenceException("Canvas not found!");
            }

            Type dialogType = typeof(T);
            if (_dialogs.ContainsKey(dialogType)) {
                Debug.LogWarning("Cannot create dialog twice!");
                return _dialogs[dialogType].GetComponent<T>();
            }

            T loadedDialog = await _resourceService.InstantiateAsync<T>(_currentCanvas!.transform);
            _dialogs.Add(dialogType, loadedDialog.gameObject);
            return loadedDialog;
        }

        public UniTask HideDialogAsync<T>()
                where T : Component
        {
            if (_currentCanvas == null) {
                Debug.LogWarning("Canvas already destroyed, no need to hide dialog!");
                return UniTask.CompletedTask;
            }

            Type dialogType = typeof(T);
            if (!_dialogs.TryGetValue(dialogType, out GameObject dialog)) {
                Debug.LogWarning($"Dialog with type={dialogType.Name} not found!");
                return UniTask.CompletedTask;
            }

            _resourceService.ReleaseInstance(dialog);
            _dialogs.Remove(dialogType);
            return UniTask.CompletedTask;
        }

        public async UniTask<T> ShowElementAsync<T>()
                where T : Component
        {
            if (_currentCanvas == null) {
                Debug.LogWarning("Canvas not found!");
                return null;
            }

            T loadedElement = await _resourceService.InstantiateAsync<T>(_currentCanvas!.transform);
            _elements.Add(loadedElement.gameObject);
            return loadedElement;
        }

        public UniTask RemoveElementAsync(GameObject element)
        {
            if (_currentCanvas == null) {
                Debug.LogWarning("Canvas already destroyed, no need to hide dialog!");
                return UniTask.CompletedTask;
            }

            if (!_elements.Contains(element)) {
                Debug.LogWarning($"Element not found to remove. ElementName ={element.name}!");
                return UniTask.CompletedTask; 
            }

            _resourceService.ReleaseInstance(element);
            _elements.Remove(element);
            return UniTask.CompletedTask;
        }
    }
}