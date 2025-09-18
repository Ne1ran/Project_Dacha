using System;
using System.Reflection;
using UnityEngine;

namespace Core.Resources.Binding.Binding
{
    internal class ComponentBinding : BindingModel, IBinding
    {
        private Type ComponentType { get; }

        public ComponentBinding(string name, MemberInfo memberInfo, Type componentType) : base(name, memberInfo)
        {
            ComponentType = componentType;
        }

        public void Bind(GameObject prefab, MonoBehaviour component, bool isParent = true)
        {
            if (!typeof(Component).IsAssignableFrom(ComponentType)) {
                throw new ArgumentException($"Bad component type '{ComponentType.Name}' for object='{Name}'" 
                                            + $" in prefab='{prefab.name}' at behaviour='{component.GetType().Name}'");
            }

            Component? childComponent = (string.IsNullOrEmpty(Name) ? prefab : GetChildByName(prefab, Name))?.GetComponent(ComponentType);
            if (childComponent == null) {
                throw new ArgumentException($"Not found component for child name='{Name}' and type='{ComponentType.Name}' "
                                            + $" in prefab='{prefab.name}' at behaviour='{component.GetType().Name}'");
            }
            
            SetField(component, childComponent);
        }
    }
}