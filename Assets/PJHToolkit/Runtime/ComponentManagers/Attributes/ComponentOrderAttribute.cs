using System;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ComponentOrderAttribute : Attribute
    {
        public int Order { get; }

        public ComponentOrderAttribute(int order)
        {
            Order = order;
        }
    }
}