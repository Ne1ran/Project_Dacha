using System;
using JetBrains.Annotations;

namespace Core.Resources.Binding.Attributes
{
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ObjectBindingAttribute : Attribute
    {
        public string ObjectName { get; }

        public ObjectBindingAttribute(string objectName) => ObjectName = objectName;
    }
}