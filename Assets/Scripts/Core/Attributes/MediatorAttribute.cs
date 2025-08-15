using System;
using JetBrains.Annotations;

namespace Core.Attributes
{
    [MeansImplicitUse, AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MediatorAttribute : Attribute
    {
        
    }  
}