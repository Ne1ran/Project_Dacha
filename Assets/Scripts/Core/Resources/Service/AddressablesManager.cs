using System.Threading;
using Core.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Addressables.Service
{
    [Service]
    public class AddressablesManager
    {
        public GameObject Instantiate(GameObject baseGameObject)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<GameObject> InstantiateAsync(object key, Transform? parent = null, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<T> LoadAssetAsync<T>(object key, CancellationToken token)
                where T : Object
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseInstance(GameObject go)
        {
            throw new System.NotImplementedException();
        }

        public void Release(Object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}