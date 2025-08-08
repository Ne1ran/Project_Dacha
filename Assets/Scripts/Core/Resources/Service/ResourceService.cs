using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Resources.Binding.Attributes;
using Core.Resources.Binding.Binding;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using VContainer.Unity;
using AppContext = Core.Scopes.AppContext;
using Object = UnityEngine.Object;

namespace Core.Resources.Service
{
    [UsedImplicitly]
    public class ResourceService : IResourceService
    {
        private readonly Dictionary<Type, PrefabBinding> _binders = new();

        private Dictionary<Type, PrefabBinding> Binders
        {
            get
            {
                if (_binders.Count == 0) {
                    InitBinders();
                }
                return _binders;
            }
        }

        public T Instantiate<T>(Transform? parent = null)
                where T : Component
        {
            string prefabPath = GetPrefabPath(typeof(T));
            if (string.IsNullOrEmpty(prefabPath)) {
                throw new InvalidOperationException("Error getting prefab path by PrefabPathAttribute");
            }

            GameObject prefab = UnityEngine.Resources.Load<GameObject>(prefabPath);
            if (prefab == null) {
                throw new InvalidOperationException($"Prefab not found with path={prefabPath}");
            }
            return DoBind<T>(prefab, parent);
        }

        public T Instantiate<T>(string prefabPath, Transform? parent = null)
                where T : Component
        {
            GameObject prefab = UnityEngine.Resources.Load<GameObject>(prefabPath);
            if (prefab == null) {
                throw new InvalidOperationException($"Prefab not found with path={prefabPath}");
            }
            return DoBind<T>(prefab, parent);
        }

        public UniTask<T> LoadObjectAsync<T>(Transform? parent = null)
                where T : Component
        {
            return UniTask.FromResult(Instantiate<T>(parent));
        }

        public UniTask<T> LoadObjectAsync<T>(string prefabPath, Transform? parent = null)
                where T : Component
        {
            return UniTask.FromResult(Instantiate<T>(prefabPath, parent));
        }

        public void Release(GameObject obj)
        {
            Object.Destroy(obj);
        }

        public void Release<T>(T obj)
                where T : Component
        {
            Object.Destroy(obj);
        }

        public T DoBind<T>(GameObject prefab, Transform? parent = null)
                where T : Component
        {
            prefab.SetActive(false);
            GameObject instantiated = Object.Instantiate(prefab, parent);
            AppContext.CurrentScope.Container.InjectGameObject(instantiated); // todo neiran temporary workaround. redo!
            if (Binders.TryGetValue(typeof(T), out PrefabBinding binding)) {
                binding.Bind(instantiated);
            }

            instantiated.SetActive(true);
            return instantiated.GetComponent<T>();
        }

        private string? GetPrefabPath(Type type)
        {
            PrefabPathAttribute prefabPathAttribute = type.GetCustomAttribute<PrefabPathAttribute>();
            return prefabPathAttribute?.PrefabPath;
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