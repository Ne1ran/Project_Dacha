using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Resources.Binding.Attributes;
using UnityEngine;

namespace Core.Resources.Binding.Binding
{
    internal class PrefabBinding : IBinding
    {
        private readonly List<ObjectBinding> _objectBindings = new List<ObjectBinding>();
        private readonly List<ComponentBinding> _componentBindings = new List<ComponentBinding>();
        private Type PrefabType { get; }

        public PrefabBinding(Type type)
        {
            PrefabType = type;
            InitBinder();
        }

        public void Bind(GameObject prefab, MonoBehaviour component = null, bool isParent = true)
        {
            component = (MonoBehaviour) (prefab.GetComponent(PrefabType) ?? prefab.AddComponent(PrefabType));
            _objectBindings.ForEach(o => o.Bind(prefab, component));
            _componentBindings.ForEach(c => c.Bind(prefab, component));

            if (!isParent) {
                return;
            }

            prefab.GetComponentsInChildren<Transform>(true)
                  .Where(c => c.gameObject != prefab)
                  .ToList()
                  .ForEach(c => {
                      List<MonoBehaviour> components = new List<MonoBehaviour>();
                      c.GetComponents(components);
                      components.ForEach(o => {
                          if (PrefabBinder.Binders.TryGetValue(o.GetType(), out PrefabBinding binding)) {
                              binding.Bind(c.gameObject, isParent: false);
                          }
                      });
                  });
        }

        private void InitBinder()
        {
            foreach (FieldInfo fieldInfo in GetFields()) {
                foreach (Attribute attribute in fieldInfo.GetCustomAttributes(false)) {
                    switch (attribute) {
                        case ComponentBindingAttribute componentAttr:
                            _componentBindings.Add(new ComponentBinding(componentAttr.ComponentName, fieldInfo, fieldInfo.FieldType));
                            break;
                        case ObjectBindingAttribute objectAttr:
                            _objectBindings.Add(new ObjectBinding(objectAttr.ObjectName, fieldInfo));
                            break;
                    }
                }
            }
        }

        private List<FieldInfo> GetFields()
        {
            List<FieldInfo> result = new List<FieldInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type currentType = PrefabType;

            while (currentType != null) {
                foreach (FieldInfo fieldInfo in currentType.GetFields(flags)) {
                    if (result.All(f => f.Name != fieldInfo.Name)) {
                        result.Add(fieldInfo);
                    }
                }

                flags = BindingFlags.NonPublic | BindingFlags.Instance;
                currentType = currentType.BaseType;
            }

            return result;
        }
    }
}