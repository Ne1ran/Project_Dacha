using System;
using UnityEngine;

namespace Core.Resources.Binding.Extensions
{
    public static class PrefabBindingExtensions
    {
        public static T TryBind<T>(this GameObject prefab, Transform parent = null)
                where T : MonoBehaviour
        {
            T component = prefab?.GetComponent<T>();
            if (component == null) {
                throw new ArgumentException($"Not found type '{typeof(T)}' for prefab binding. Prefab name={prefab?.name}");
            }

            return PrefabBinder.DoBind<T>(prefab, parent);
        }
    }
}