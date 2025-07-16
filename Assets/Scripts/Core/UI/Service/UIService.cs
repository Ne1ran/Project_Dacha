using Core.Scene.Event;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.UI.Service
{
    public class UIService : IInitializable
    {
        [Inject]
        private ISubscriber<string, SceneChangedEvent> _subscriber;

        public void Initialize()
        {
            _subscriber.Subscribe(SceneChangedEvent.SCENE_LOADED, OnSceneChanged);
        }

        private void OnSceneChanged(SceneChangedEvent evt)
        {
            Debug.LogError("    Scene has been changed   ");
        }
    }
}