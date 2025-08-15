using System;
using JetBrains.Annotations;

namespace Game.Common.Handlers
{
    [MeansImplicitUse, AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class HandlerAttribute : Attribute
    {
        public string Name { get; }

        public HandlerAttribute(string name)
        {
            Name = name;
        }
    }
}