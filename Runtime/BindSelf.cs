using System;
using UnityEngine;

namespace Bindables
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindSelfAttribute : PropertyAttribute
    {
        public BindSelfAttribute() { }
    }
}