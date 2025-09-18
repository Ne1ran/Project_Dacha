using System;
using System.Threading;
using Core.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.Resources.Service
{
    [Service]
    public class AddressablesManager
    {
        public async UniTask InitializeAsync()
        {
            AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
            await handle;
            switch (handle.Status) {
                case AsyncOperationStatus.Succeeded:
                    Debug.Log("Addressables initialized successfully!");
                    break;
                case AsyncOperationStatus.Failed:
                    throw new("Addressables initialization failed!");
                default:
                    throw new("Addressables initialization returned none!");
            }
        }

        public GameObject Instantiate(GameObject baseGameObject)
        {
            return Object.Instantiate(baseGameObject);
        }

        public async UniTask<GameObject> InstantiateAsync(object key, Transform? parent = null, CancellationToken cancellationToken = default)
        {
            GameObject loadedObj = await Addressables.InstantiateAsync(key);
            if (cancellationToken.IsCancellationRequested) {
                throw new OperationCanceledException();
            }

            Transform newObjTransform = loadedObj.transform;
            if (parent != null) {
                newObjTransform.SetParent(parent, false);
            }

            return loadedObj;
        }

        public async UniTask<GameObject> InstantiateAsync(object key,
                                                          Vector3 position,
                                                          Quaternion rotation,
                                                          CancellationToken cancellationToken = default)
        {
            GameObject loadedObj = await Addressables.InstantiateAsync(key);
            if (cancellationToken.IsCancellationRequested) {
                throw new OperationCanceledException();
            }

            Transform loadedObjTransform = loadedObj.transform;
            loadedObjTransform.position = position;
            loadedObjTransform.rotation = rotation;
            return loadedObj;
        }

        public async UniTask<T> LoadAssetAsync<T>(object key, CancellationToken token)
                where T : Object
        {
            T? loadedAsset = await Addressables.LoadAssetAsync<T>(key);
            if (token.IsCancellationRequested) {
                throw new OperationCanceledException();
            }

            return loadedAsset;
        }

        public void ReleaseInstance(GameObject go)
        {
            Addressables.ReleaseInstance(go);
        }

        public void Release(Object obj)
        {
            Addressables.Release(obj);
        }
    }
}