using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Descriptors.Service
{
    public interface IDescriptorService
    {
        UniTask InitializeAsync();
        
        T Require<T>() where T : ScriptableObject;
    }
}