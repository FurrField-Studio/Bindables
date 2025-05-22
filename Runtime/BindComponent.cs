using System;
using UnityEngine;

namespace Bindables
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindComponentAttribute : BindAttribute
    {
        public string FilterMethod { get; private set; }

        public BindComponentAttribute() { }

        public BindComponentAttribute(string name = null, string filterMethod = null) : base(name)
        {
            FilterMethod = filterMethod;
        }
    }
}