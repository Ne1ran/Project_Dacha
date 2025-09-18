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
                    NeedBindingAttribute attribute = type.GetCustomAttribute<NeedBindingAttribute>();
                    if (attribute == null) {
                        continue;
                    }

                    _binders[type] = new(type);
                }
            }
        }
    }
}