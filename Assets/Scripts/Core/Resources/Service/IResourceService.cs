using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Resources.Service
{
    public interface IResourceService
    {
        public GameObject? Instantiate(GameObject baseGameObject);

        public T? Instantiate<T>(GameObject baseObject, Transform? parent = null) where T : Component;

        public UniTask<T> InstantiateAsync<T>(string key, Transform parent, CancellationToken token = default) where T : Component;

        public UniTask<List<T>> InstantiateAsync<T>(string key, int instancesCount, CancellationToken token = default) where T : Component;

        public UniTask<GameObject> InstantiateAsync(string key, Transform parent, CancellationToken token = default);

        public UniTask<List<GameObject>> InstantiateAsync(string key, int instancesCount, CancellationToken token = default);

        public UniTask<T> InstantiateAsync<T>(string key, Vector3? position = null, Quaternion? rotation = null, CancellationToken token = default) 
                where T : Component;

        public UniTask<GameObject> InstantiateAsync(string key,
                                                    Vector3? position = null,
                                                    Quaternion? rotation = null,
                                                    CancellationToken token = default);

        public UniTask<T> InstantiateAsync<T>(Transform parent, CancellationToken token = default) where T : Component;

        public UniTask<List<T>> InstantiateAsync<T>(int instancesCount, CancellationToken token = default)
                where T : Component;

        public UniTask<T> InstantiateAsync<T>(Vector3? position = null, Quaternion? rotation = null, CancellationToken token = default)
                where T : Component;

        public UniTask<T> LoadAssetAsync<T>(string key, CancellationToken token = default) where T : Object;

        public void ReleaseInstance(GameObject go);

        public void Release(Component component);

        public void Release(Object obj);
    }
}