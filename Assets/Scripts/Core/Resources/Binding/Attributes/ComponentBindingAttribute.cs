using System;
using JetBrains.Annotations;

namespace Core.Resources.Binding.Attributes
{
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ComponentBindingAttribute : Attribute
    {
        [CanBeNull] 
        public string ComponentName { get; }
        
        public ComponentBindingAttribute(string componentName) => ComponentName = componentName;

        public ComponentBindingAttribute() : this(null)
        {
        }
    }
}