using Core.Attributes;
using Core.Console.Controller;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Console.Service
{
    [Service]
    public class ConsoleService
    {
        private readonly IObjectResolver _objectResolver;
        private readonly PrefabBinderManager _prefabBinderManager;

        public ConsoleService(IObjectResolver objectResolver, PrefabBinderManager prefabBinderManager)
        {
            _objectResolver = objectResolver;
            _prefabBinderManager = prefabBinderManager;
        }

        public UniTask InitializeAsync()
        {
            GameObject? consoleAsset = UnityEngine.Resources.Load<GameObject>("pfDebugConsole");
            if (consoleAsset == null) {
                Debug.LogWarning("ConsoleController not found!");
            }

            GameObject consoleObj = Object.Instantiate(consoleAsset)!;
            _objectResolver.InjectGameObject(consoleObj);
            _prefabBinderManager.DoBind<ConsoleController>(consoleObj);
            return UniTask.CompletedTask;
        }
    }
}