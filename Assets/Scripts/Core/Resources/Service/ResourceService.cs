using System;
using System.Collections.Generic;
using System.Threading;
using Core.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Core.Resources.Service
{
    [Service]
    public class ResourceService : IResourceService
    {
        private readonly IObjectResolver _objectResolver;
        private readonly PrefabBinderManager _prefabBinderManager;

        public ResourceService(IObjectResolver objectResolver, PrefabBinderManager prefabBinderManager)
        {
            _objectResolver = objectResolver;
            _prefabBinderManager = prefabBinderManager;
        }

        public GameObject Instantiate(GameObject baseGameObject)
        {
            GameObject newObj = Object.Instantiate(baseGameObject);
            _objectResolver.InjectGameObject(newObj);
            return newObj;
        }

        public T Instantiate<T>(GameObject baseObject, Transform? parent = null)
                where T : Component
        {
            GameObject newObj = Instantiate(baseObject);
            if (parent != null) {
                newObj.transform.SetParent(parent);
            }

            return _prefabBinderManager.DoBind<T>(newObj);
        }

        public UniTask<T> InstantiateAsync<T>(string key, Transform parent, CancellationToken token = default)
                where T : Component
        {
            return LoadObjectAsync<T>(key, parent, token);
        }

        public UniTask<List<T>> InstantiateAsync<T>(string key, int instancesCount, CancellationToken token = default)
                where T : Component
        {
            List<T> result = new(instancesCount);
            for (int i = 0; i < instancesCount; i++) {
                result.Add(LoadComponent<T>(key));
            }

            return UniTask.FromResult(result);
        }

        public UniTask<GameObject> InstantiateAsync(string key, Transform parent, CancellationToken token = default)
        {
            return UniTask.FromResult(LoadObject(key, parent));
        }

        public UniTask<List<GameObject>> InstantiateAsync(string key, int instancesCount, CancellationToken token = default)
        {
            List<GameObject> result = new(instancesCount);
            for (int i = 0; i < instancesCount; i++) {
                result.Add(LoadObject(key));
            }

            return UniTask.FromResult(result);
        }

        public async UniTask<T> InstantiateAsync<T>(string key,
                                                    Vector3? position = null,
                                                    Quaternion? rotation = null,
                                                    CancellationToken token = default)
                where T : Component
        {
            T loadedObj = await LoadObjectAsync<T>(key, token: token);

            if (position == null && rotation == null) {
                return loadedObj;
            }

            loadedObj.transform.position = position ?? Vector3.zero;
            loadedObj.transform.rotation = rotation ?? Quaternion.identity;
            return loadedObj;
        }

        public UniTask<GameObject> InstantiateAsync(string key,
                                                    Vector3? position = null,
                                                    Quaternion? rotation = null,
                                                    CancellationToken token = default)
        {
            GameObject loadedObj = LoadObject(key);
            if (position == null && rotation == null) {
                return UniTask.FromResult(loadedObj);
            }

            loadedObj.transform.position = position ?? Vector3.zero;
            loadedObj.transform.rotation = rotation ?? Quaternion.identity;
            return UniTask.FromResult(loadedObj);
        }

        public UniTask<T> InstantiateAsync<T>(Transform parent, CancellationToken token = default)
                where T : Component
        {
            string path = _prefabBinderManager.RequireBindingPath<T>();
            return InstantiateAsync<T>(path, parent, token);
        }

        public UniTask<List<T>> InstantiateAsync<T>(int instancesCount, CancellationToken token = default)
                where T : Component
        {
            string path = _prefabBinderManager.RequireBindingPath<T>();
            return InstantiateAsync<T>(path, instancesCount, token);
        }

        public UniTask<T> InstantiateAsync<T>(Vector3? position = null, Quaternion? rotation = null, CancellationToken token = default)
                where T : Component
        {
            string path = _prefabBinderManager.RequireBindingPath<T>();
            return InstantiateAsync<T>(path, position, rotation, token);
        }

        public UniTask<T> LoadAssetAsync<T>(string key, CancellationToken token = default)
                where T : Object
        {
            return UniTask.FromResult(UnityEngine.Resources.Load<T>(key));
        }

        public void ReleaseInstance(GameObject go)
        {
            Object.Destroy(go);
        }

        public void Release(Component component)
        {
            Object.Destroy(component);
        }

        public void Release(Object obj)
        {
            Object.Destroy(obj);
        }

        private UniTask<T> LoadObjectAsync<T>(string prefabPath, Transform? parent = null, CancellationToken token = default)
                where T : Component
        {
            return UniTask.FromResult(LoadComponent<T>(prefabPath, parent));
        }

        private T LoadComponent<T>(string prefabPath, Transform? parent = null)
                where T : Component
        {
            GameObject prefab = LoadObject(prefabPath, parent);
            return _prefabBinderManager.DoBind<T>(prefab);
        }

        private GameObject LoadObject(string prefabPath, Transform? parent = null)
        {
            GameObject prefab = UnityEngine.Resources.Load<GameObject>(prefabPath);
            if (prefab == null) {
                throw new InvalidOperationException($"Prefab not found with path={prefabPath}");
            }

            GameObject instantiated = Object.Instantiate(prefab, parent);
            if (parent != null) {
                instantiated.transform.SetParent(parent);
            }

            _objectResolver.InjectGameObject(instantiated);
            return instantiated;
        }
    }
}