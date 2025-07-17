using System;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : UsedImplicitlyAttribute { }
}