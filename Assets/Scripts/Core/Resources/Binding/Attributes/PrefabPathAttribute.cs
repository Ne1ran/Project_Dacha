using System;
using JetBrains.Annotations;

namespace Core.Resources.Binding.Attributes
{
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Class)]
    public class PrefabPathAttribute : Attribute
    {
        public string PrefabPath { get; }

        public PrefabPathAttribute(string prefabPath) => PrefabPath = prefabPath;
    }
}