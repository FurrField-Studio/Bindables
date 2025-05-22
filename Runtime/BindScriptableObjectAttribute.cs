using System;
using UnityEngine;

namespace Bindables
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindScriptableObjectAttribute : BindAttribute
    {
        public string FilterMethod { get; private set; }
        public string Prefix { get; private set; }
        public string Path { get; private set; }

        public BindScriptableObjectAttribute() { }
        
        public BindScriptableObjectAttribute(string name = null, string filterMethod = null, string prefix = null, string path = null) : base(name)
        {
            FilterMethod = filterMethod;
            Prefix = prefix;
            Path = path;
        }
    }
}