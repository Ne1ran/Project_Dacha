using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Utils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<(Type, TAttribute)> FindAllTypesWithAttribute<TAttribute>(IReadOnlyList<string> assemblyNames)
                where TAttribute : Attribute
        {
            return GetTypesFromAssemblies(assemblyNames).Select(t => (t, t.GetCustomAttribute<TAttribute>())).Where(t => t.Item2 != null);
        }

        public static IEnumerable<Type> FindAllTypesWithInterface<TInterface>(IReadOnlyList<string> assemblyNames)
        {
            return GetTypesFromAssemblies(assemblyNames).Where(t => t.GetInterfaces().Contains(typeof(TInterface)));
        }

        public static IEnumerable<(Type, TAttribute)> FindAllTypesWithAttributeAndInterface<TAttribute, TInterface>(
                IReadOnlyList<string> assemblyNames)
                where TAttribute : Attribute
        {
            return GetTypesFromAssemblies(assemblyNames)
                   .Where(t => t.GetInterfaces().Contains(typeof(TInterface)))
                   .Select(t => (t, t.GetCustomAttribute<TAttribute>()))
                   .Where(t => t.Item2 != null);
        }

        public static IEnumerable<(Type, TAttribute)> FindAllTypesWithAttributeAndBaseType<TAttribute, TBase>(IReadOnlyList<string> assemblyNames)
                where TAttribute : Attribute
        {
            return GetTypesFromAssemblies(assemblyNames)
                   .Where(t => {
                       Type? baseType = t.BaseType;
                       while (baseType != null) {
                           if (baseType == typeof(TBase)) {
                               return true;
                           }
                           baseType = baseType.BaseType;
                       }
                       return false;
                   })
                   .Select(t => (t, t.GetCustomAttribute<TAttribute>()))
                   .Where(t => t.Item2 != null);
        }

        private static IEnumerable<Type> GetTypesFromAssemblies(IReadOnlyList<string> assemblyNames)
        {
            return assemblyNames.Select(Assembly.Load).SelectMany(a => a.GetTypes());
        }

        public static TAttribute RequireAttribute<TAttribute>(Type sourceType)
                where TAttribute : Attribute
        {
            TAttribute attribute = sourceType.GetCustomAttribute<TAttribute>();
            return attribute
                   ?? throw new
                           NullReferenceException($"There is no attribute on specified type. SourceType={sourceType.Name}. AttributeType={typeof(TAttribute).Name}");
        }

        public static bool IsInterfaceImplemented<TInterface>(this Type type)
        {
            Type interfaceType = typeof(TInterface);

            if (!interfaceType.IsInterface) {
                throw new ArgumentException($"Generic argument have to be interface type. Type={interfaceType.Name}");
            }

            if (interfaceType == type) {
                return true;
            }

            foreach (Type? @interface in type.GetInterfaces()) {
                if (@interface == interfaceType) {
                    return true;
                }
            }

            return false;
        }
    }
}