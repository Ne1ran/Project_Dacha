using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Resources.Service
{
    public class AddressablesResourceService : IResourceService
    {
        public T Instantiate<T>(Transform parent = null)
                where T : MonoBehaviour
        {
            throw new System.NotImplementedException();
        }

        public UniTask<T> LoadObjectAsync<T>(Transform parent = null)
                where T : MonoBehaviour
        {
            throw new System.NotImplementedException();
        }

        public void Release(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        public void Release<T>(T obj)
                where T : MonoBehaviour
        {
            throw new System.NotImplementedException();
        }
    }
}