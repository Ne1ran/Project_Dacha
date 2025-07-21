using UnityEngine;

namespace Core.Descriptors.Service
{
    public interface IDescriptorService
    {
        T LoadDescriptorAsync<T>() where T : ScriptableObject;
    }
}