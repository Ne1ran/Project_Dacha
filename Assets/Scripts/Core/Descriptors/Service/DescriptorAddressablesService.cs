using UnityEngine;

namespace Core.Descriptors.Service
{
    public class DescriptorAddressablesService : IDescriptorService
    {
        public T LoadDescriptorAsync<T>()
                where T : ScriptableObject
        {
            throw new System.NotImplementedException();
        }
    }
}