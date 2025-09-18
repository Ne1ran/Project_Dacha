using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Resources.Binding.Attributes;
using Core.Resources.Binding.Binding;

namespace Core.Resources.Binding
{
    public static class PrefabBinder
    {
        internal static Dictionary<Type, PrefabBinding> Binders { get; } = new();

        public static void InitBinders()
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

                    Binders[type] = new(type);
                }
            }
        }
    }
}