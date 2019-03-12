using System;
using System.Collections.Generic;
using System.Text;

namespace SharedNavigation.NETStandard
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class NavigateAttribute : Attribute
    {
        public string InjectPropertyName { get; }

        public NavigateAttribute(string injectPropertyName)
        {
            InjectPropertyName = injectPropertyName;
        }
    }
}
