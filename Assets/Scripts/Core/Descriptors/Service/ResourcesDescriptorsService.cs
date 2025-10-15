using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Attributes;
using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Descriptors.Service
{
    [Service]
    public class ResourcesDescriptorsService : IDescriptorService
    {
        private readonly Dictionary<Type, ScriptableObject> _cachedDescriptors = new();

        public UniTask InitializeAsync()
        {
            List<Type> typesWithDescriptor = new();

            HashSet<Assembly> assemblies = new() {
                    Assembly.GetExecutingAssembly()
            };

            try {
                assemblies.Add(Assembly.Load("Assembly-CSharp"));
            } catch {
                // ignored
            }
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    DescriptorAttribute attribute = type.GetCustomAttribute<DescriptorAttribute>();
                    if (attribute == null) {
                        continue;
                    }
                    
                    typesWithDescriptor.Add(type);
                }
            }

            foreach (Type type in typesWithDescriptor)
            {
                DescriptorAttribute descriptorAttribute = ReflectionUtils.RequireAttribute<DescriptorAttribute>(type);
                Object? descObj = UnityEngine.Resources.Load(descriptorAttribute.DescriptorPath, type);
                if (descObj == null) {
                    throw new ArgumentException($"Descriptor not found on path={descriptorAttribute.DescriptorPath}");
                }

                ScriptableObject? descriptor = descObj as ScriptableObject;
                if (descriptor == null) {
                    throw new ArgumentException($"Descriptor is not castable on path={descriptorAttribute.DescriptorPath}");
                }
                
                _cachedDescriptors.TryAdd(type, descriptor);
            }

            return UniTask.CompletedTask;
        }

        public T Require<T>()
                where T : ScriptableObject
        {
            Type type = typeof(T);
            if (_cachedDescriptors.TryGetValue(type, out ScriptableObject cachedDescriptor)) {
                return (T) cachedDescriptor;
            }
            
            Debug.LogWarning($"Descriptor was not loaded earlier. Will try to load manually. Descriptor type={nameof(type)}");
            
            DescriptorAttribute descriptorAttribute = ReflectionUtils.RequireAttribute<DescriptorAttribute>(type);
            T descriptor = UnityEngine.Resources.Load<T>(descriptorAttribute.DescriptorPath);
            if (descriptor == null) {
                throw new ArgumentException($"Descriptor not found on path={descriptorAttribute.DescriptorPath}");
            }
            
            _cachedDescriptors.Add(type, descriptor);
            return descriptor;
        }
    }
}