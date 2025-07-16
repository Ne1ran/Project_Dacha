using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Resources.Binding.Attributes;
using Core.Resources.Binding.Binding;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Core.Resources.Service
{
    public class ResourceService : IResourceService
    {
        private readonly IObjectResolver _objectResolver;

        public ResourceService(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        private readonly Dictionary<Type, PrefabBinding> _binders = new();

        [NotNull]
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

        public T Instantiate<T>([CanBeNull] Transform parent = null)
                where T : MonoBehaviour
        {
            string prefabPath = GetPrefabPath(typeof(T));
            if (string.IsNullOrEmpty(prefabPath)) {
                throw new InvalidOperationException("Error getting prefab path by PrefabPathAttribute");
            }

            GameObject prefab = UnityEngine.Resources.Load<GameObject>(prefabPath);
            return DoBind<T>(prefab, parent);
        }

        public UniTask<T> LoadObjectAsync<T>([CanBeNull] Transform parent = null)
                where T : MonoBehaviour
        {
            return UniTask.FromResult(Instantiate<T>(parent));
        }

        public void Release(GameObject obj)
        {
            Object.Destroy(obj);
        }

        public void Release<T>(T obj)
                where T : MonoBehaviour
        {
            Object.Destroy(obj);
        }

        public T DoBind<T>(GameObject prefab, [CanBeNull] Transform parent = null)
                where T : MonoBehaviour
        {
            bool activeSelf = prefab.activeSelf;
            prefab.SetActive(false);
            GameObject instantiated = Object.Instantiate(prefab, parent);
            _objectResolver.InjectGameObject(instantiated);

            if (!Binders.TryGetValue(typeof(T), out PrefabBinding binding)) {
                throw new ArgumentException($"Not found type '{typeof(T)}' for prefab binding.");
            }

            binding.Bind(instantiated);
            instantiated.SetActive(activeSelf);
            return instantiated.GetComponent<T>();
        }

        [CanBeNull]
        private string GetPrefabPath(Type type)
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