using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Descriptors.Service
{
    public class DescriptorAddressablesService : IDescriptorService
    {
        public UniTask InitializeAsync()
        {
            throw new System.NotImplementedException();
        }

        public T Require<T>()
                where T : ScriptableObject
        {
            throw new System.NotImplementedException();
        }
    }
}