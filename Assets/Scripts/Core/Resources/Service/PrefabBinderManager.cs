using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Resources.Binding.Attributes;
using Core.Resources.Binding.Binding;
using UnityEngine;
using VContainer.Unity;

namespace Core.Resources.Service
{
    [Serializable]
    public class PrefabBinderManager : IInitializable
    {
        private readonly Dictionary<Type, PrefabBinding> _binders = new();

        public void Initialize()
        {
            InitBinders();
        }

        public T DoBind<T>(GameObject prefab)
                where T : Component
        {
            if (Binders.TryGetValue(typeof(T), out PrefabBinding binding)) {
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

        private void InitBinders()
        {
            HashSet<Assembly> assemblies = new() {
                    Assembly.GetExecutingAssembly()
            };

            try {
                assemblies.Add(Assembly.Load("Assembly-CSharp"));
            } catch {
                // ignored
            }

            foreach (Assembly assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    NeedBindingAttribute attribute = type.GetCustomAttribute<NeedBindingAttribute>();
                    if (attribute == null) {
                        continue;
                    }

                    _binders[type] = new(type);
                }
            }
        }

        private Dictionary<Type, PrefabBinding> Binders => _binders;
    }
}