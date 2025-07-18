using System;
using System.Collections.Generic;
using Core.Resources.Service;
using Core.Scene.Event;
using Core.Scene.Service;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.UI.Service
{
    public class UIService : IInitializable, IDisposable
    {
        [Inject]
        private ISubscriber<string, SceneChangedEvent> _subscriber;
        [Inject]
        private IResourceService _resourceService;
        [Inject]
        private SceneService _sceneService;

        [CanBeNull]
        private Canvas _currentCanvas;

        //todo neiran add priority with dialogs

        private readonly Dictionary<Type, GameObject> _dialogs = new();

        [CanBeNull]
        private IDisposable _disposable;

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
                Debug.LogWarning("Canvas not found!");
                return null;
            }

            Type dialogType = typeof(T);
            if (_dialogs.ContainsKey(dialogType)) {
                Debug.LogWarning("Cannot create dialog twice!");
                return null;
            }

            T loadedDialog = await _resourceService.LoadObjectAsync<T>(_currentCanvas!.transform);
            _dialogs.Add(dialogType, loadedDialog.gameObject);
            return loadedDialog;
        }

        public UniTask HideDialogAsync<T>()
                where T : Component
        {
            if (_currentCanvas == null) {
                Debug.LogWarning("Canvas already destroyed, no need to hide dialog!");
                return UniTask.CompletedTask;
                ;
            }

            Type dialogType = typeof(T);
            if (!_dialogs.TryGetValue(dialogType, out GameObject dialog)) {
                Debug.LogWarning($"Dialog with type={dialogType.Name} not found!");
                return UniTask.CompletedTask;
                ;
            }

            _resourceService.Release(dialog);
            _dialogs.Remove(dialogType);
            return UniTask.CompletedTask;
        }
    }
}