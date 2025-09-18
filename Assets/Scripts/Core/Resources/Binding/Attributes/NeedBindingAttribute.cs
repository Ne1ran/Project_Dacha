using System;
using JetBrains.Annotations;

namespace Core.Resources.Binding.Attributes
{
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Class)]
    public class NeedBindingAttribute : Attribute
    {
        public string Path { get; }

        public NeedBindingAttribute(string path) => Path = path;
    }
}