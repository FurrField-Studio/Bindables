using System;
using UnityEngine;

namespace Bindables
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindAttribute : PropertyAttribute
    {
        public string Name { get; private set; }

        public BindAttribute() { }

        public BindAttribute(string name = null)
        {
            Name = name;
        }
    }
}