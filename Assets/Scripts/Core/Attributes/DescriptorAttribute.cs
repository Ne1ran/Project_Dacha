using System;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptorAttribute : Attribute
    {
        public string DescriptorPath { get; }

        public DescriptorAttribute(string descriptorPath)
        {
            DescriptorPath = descriptorPath;
        }
    }
}