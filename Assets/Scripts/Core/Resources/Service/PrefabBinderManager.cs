using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Attributes;
using Core.Resources.Binding;
using Core.Resources.Binding.Attributes;
using Core.Resources.Binding.Binding;
using UnityEngine;
using VContainer.Unity;

namespace Core.Resources.Service
{
    [Service]
    public class PrefabBinderManager : IInitializable
    {
        public void Initialize()
        {
            PrefabBinder.InitBinders();
        }

        public T DoBind<T>(GameObject prefab)
                where T : Component
        {
            if (PrefabBinder.Binders.TryGetValue(typeof(T), out PrefabBinding binding)) {
                binding.Bind(prefab);
            }

            prefab.SetActive(true);
            return prefab.GetComponent<T>();
        }

        public string RequireBindingPath<T>()
                where T : Component
        {
            string? path = GetBindingPath(typeof(T));
            if (string.IsNullOrEmpty(path)) {
                throw new KeyNotFoundException($"Path not found on prefab with type={typeof(T)}");
            }

            return path;
        }

        public string? GetBindingPath(Type type)
        {
            NeedBindingAttribute prefabPathAttribute = type.GetCustomAttribute<NeedBindingAttribute>();
            return prefabPathAttribute?.Path;
        }
    }
}