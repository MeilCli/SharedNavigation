using System;
using System.Collections.Generic;
using System.Text;

namespace SharedNavigation.NETStandard
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class InjectNavigationAttribute : Attribute
    {
    }
}
