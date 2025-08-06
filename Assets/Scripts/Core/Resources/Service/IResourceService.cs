using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Resources.Service
{
    public interface IResourceService
    {
        T Instantiate<T>(Transform parent = null) where T : Component;

        T Instantiate<T>(string prefabPath, Transform parent = null) where T : Component;

        UniTask<T> LoadObjectAsync<T>(Transform parent = null) where T : Component;

        UniTask<T> LoadObjectAsync<T>(string path, Transform parent = null) where T : Component;

        void Release(GameObject obj);

        void Release<T>(T obj) where T : Component;
    }
}