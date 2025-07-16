using UnityEngine;

namespace Core.Resources.Binding.Binding
{
    public interface IBinding
    {
        void Bind(GameObject prefab, MonoBehaviour component, bool isParent);
    }
}