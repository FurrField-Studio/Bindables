using System;

namespace Bindables
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindGameobjectAttribute : BindAttribute
    {
        public string FilterMethod { get; private set; }

        public BindGameobjectAttribute() { }

        public BindGameobjectAttribute(string name = null, string filterMethod = null) : base(name)
        {
            FilterMethod = filterMethod;
        }
    }
}