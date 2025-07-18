using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Resources.Binding.Attributes;
using Core.Resources.Binding.Binding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Resources.Binding
{
    public static class PrefabBinder
    {
        private static readonly Dictionary<Type, PrefabBinding> _binders = new();

        internal static Dictionary<Type, PrefabBinding> Binders
        {
            get
            {
                if (_binders.Count == 0) {
                    InitBinders();
                }
                return _binders;
            }
        }

        public static T Instantiate<T>(Transform parent = null)
                where T : MonoBehaviour
        {
            string prefabPath = GetPrefabPath(typeof(T));
            if (string.IsNullOrEmpty(prefabPath)) {
                throw new InvalidOperationException("Error getting prefab path by PrefabPathAttribute");
            }

            GameObject prefab = UnityEngine.Resources.Load<GameObject>(prefabPath);
            return DoBind<T>(prefab, parent);
        }

        internal static T DoBind<T>(GameObject prefab, Transform parent = null)
                where T : MonoBehaviour
        {
            bool activeSelf = prefab.activeSelf;
            prefab.SetActive(false);
            GameObject instantiated = Object.Instantiate(prefab, parent);

            if (!Binders.TryGetValue(typeof(T), out PrefabBinding binding)) {
                throw new ArgumentException($"Not found type '{typeof(T)}' for prefab binding.");
            }

            binding.Bind(instantiated);
            instantiated.SetActive(activeSelf);
            return instantiated.GetComponent<T>();
        }

        private static string GetPrefabPath(Type type)
        {
            PrefabPathAttribute prefabPathAttribute = type.GetCustomAttribute<PrefabPathAttribute>();
            return prefabPathAttribute?.PrefabPath;
        }

        private static void InitBinders()
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
                    PrefabPathAttribute attribute = type.GetCustomAttribute<PrefabPathAttribute>();
                    if (attribute == null) {
                        continue;
                    }

                    _binders[type] = new(type);
                }
            }
        }
    }
}