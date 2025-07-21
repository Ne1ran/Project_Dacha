using System;
using System.Collections.Generic;
using Core.Attributes;
using Game.Utils;
using UnityEngine;

namespace Core.Descriptors.Service
{
    [UsedImplicitly]
    public class ResourcesDescriptorsService : IDescriptorService
    {
        private readonly Dictionary<Type, ScriptableObject> _cachedDescriptors = new();
        
        public T LoadDescriptorAsync<T>()
                where T : ScriptableObject
        {
            Type type = typeof(T);
            if (_cachedDescriptors.TryGetValue(type, out ScriptableObject cachedDescriptor)) {
                return (T) cachedDescriptor;
            }
            
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