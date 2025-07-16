using System;
using System.Reflection;
using UnityEngine;

namespace Core.Resources.Binding.Binding
{
    internal class ObjectBinding : BindingModel, IBinding
    {
        public ObjectBinding(string name, MemberInfo memberInfo) : base(name, memberInfo)
        {
        }
        
        public void Bind(GameObject prefab, MonoBehaviour component, bool isParent = true)
        {
            GameObject child = GetChildByName(prefab, Name);
            if (child == null) {
                throw new ArgumentException($"Error binding object with name '{Name}' in component '{component.gameObject.name}'");
            }

            SetField(component, child);
        }
    }
}