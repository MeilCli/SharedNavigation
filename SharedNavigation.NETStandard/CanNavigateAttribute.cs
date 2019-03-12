using System;
using System.Collections.Generic;
using System.Text;

namespace SharedNavigation.NETStandard
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CanNavigateAttribute : Attribute
    {
        public string InjectPropertyName { get; }

        public CanNavigateAttribute(string injectPropertyName)
        {
            InjectPropertyName = injectPropertyName;
        }
    }
}
