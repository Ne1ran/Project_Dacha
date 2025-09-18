using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Attributes;
using Core.Scopes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Core.Resources.Service
{
    [Service]
    public class AddressablesResourceService : IResourceService
    {
        private readonly AddressablesManager _addressablesManager;
        private readonly PrefabBinderManager _prefabBinderManager;

        public GameObject? Instantiate(GameObject baseGameObject)
        {
            GameObject? go = _addressablesManager.Instantiate(baseGameObject);
            if (go != null) {
                AppContext.CurrentScope.Container.InjectGameObject(go);
            }

            return go;
        }

        public T? Instantiate<T>(GameObject baseObject, Transform? parent = null)
                where T : Component
        {
            GameObject? go = _addressablesManager.Instantiate(baseObject.gameObject);
            if (go == null) {
                return null;
            }

            if (parent != null) {
                go.transform.SetParent(parent);
            }

            AppContext.CurrentScope.Container.InjectGameObject(go);
            return _prefabBinderManager.DoBind<T>(go);
        }

        public T? Instantiate<T>(T baseObject, Transform parent)
                where T : Component
        {
            GameObject? go = _addressablesManager.Instantiate(baseObject.gameObject);
            if (go == null) {
                return null;
            }

            if (parent != null) {
                go.transform.SetParent(parent);
            }

            AppContext.CurrentScope.Container.InjectGameObject(go);
            return _prefabBinderManager.DoBind<T>(go);
        }

        public async UniTask<T> InstantiateAsync<T>(string key, Transform parent, CancellationToken token = default)
                where T : Component
        {
            GameObject go = await InstantiateAsync(key, parent, token);
            return _prefabBinderManager.DoBind<T>(go);
        }

        public async UniTask<T> InstantiateAsync<T>(string key,
                                                    Vector3? position = null,
                                                    Quaternion? rotation = null,
                                                    CancellationToken token = default)
                where T : Component
        {
            GameObject go = await InstantiateAsync(key, position, rotation, token);
            return _prefabBinderManager.DoBind<T>(go);
        }

        public async UniTask<List<T>> InstantiateAsync<T>(string key, int instancesCount, CancellationToken token = default)
                where T : Component
        {
            List<UniTask<T>> result = new(instancesCount);
            for (int i = 0; i < instancesCount; i++) {
                result.Add(CreateInstanceAndBindAsync<T>(key, token));
            }
            T[] resultedTasks = await UniTask.WhenAll(result);
            return resultedTasks.ToList();
        }

        public async UniTask<GameObject> InstantiateAsync(string key, Transform parent, CancellationToken token = default)
        {
            GameObject go = await _addressablesManager.InstantiateAsync(key, parent, cancellationToken: token);
            AppContext.CurrentScope.Container.InjectGameObject(go);
            return go;
        }

        public async UniTask<List<GameObject>> InstantiateAsync(string key, int instancesCount, CancellationToken token = default)
        {
            List<GameObject> result = new(instancesCount);
            GameObject go = await InstantiateAsync(key, token: token);
            result.Add(go);
            for (int i = 1; i < instancesCount; i++) {
                result.Add(Instantiate(go)!);
            }

            return result;
        }

        public async UniTask<GameObject> InstantiateAsync(string key,
                                                          Vector3? position = null,
                                                          Quaternion? rotation = null,
                                                          CancellationToken token = default)
        {
            GameObject go;
            if (position == null && rotation == null) {
                go = await _addressablesManager.InstantiateAsync(key, cancellationToken: token);
            } else {
                go = await _addressablesManager.InstantiateAsync(key, position ?? Vector3.zero, rotation ?? Quaternion.identity,
                                                                 cancellationToken: token);
            }
            AppContext.CurrentScope.Container.InjectGameObject(go);
            return go;
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
            return _addressablesManager.LoadAssetAsync<T>(key, token);
        }

        public void ReleaseInstance(GameObject go)
        {
            _addressablesManager.ReleaseInstance(go);
        }

        public void Release(Component component)
        {
            _addressablesManager.ReleaseInstance(component.gameObject);
        }

        public void Release(Object obj)
        {
            _addressablesManager.Release(obj);
        }

        private async UniTask<T> CreateInstanceAndBindAsync<T>(string key, CancellationToken token = default)
                where T : Component

        {
            GameObject go = await InstantiateAsync(key, token: token);
            return _prefabBinderManager.DoBind<T>(go);
        }

        public AddressablesResourceService(AddressablesManager addressablesManager,
                                           PrefabBinderManager prefabBinderManager)
        {
            _addressablesManager = addressablesManager;
            _prefabBinderManager = prefabBinderManager;
        }
    }
}