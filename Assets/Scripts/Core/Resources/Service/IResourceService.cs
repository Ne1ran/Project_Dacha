using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Resources.Service
{
    public interface IResourceService
    {
        T Instantiate<T>(Transform parent = null)
                where T : MonoBehaviour;

        UniTask<T> LoadObjectAsync<T>(Transform parent = null)
                where T : MonoBehaviour;

        void Release(GameObject obj);

        void Release<T>(T obj)
                where T : MonoBehaviour;
    }
}