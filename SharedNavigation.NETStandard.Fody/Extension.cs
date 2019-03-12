using Mono.Cecil;
using System;

namespace SharedNavigation.NETStandard.Fody
{
    internal static class Extension
    {
        public static bool EqualType(this TypeReference type, Type target)
        {
            return type.Namespace == target.Namespace && type.Name == target.Name;
        }

        public static bool EqualGenericType(this TypeReference type, Constant.GenericType genericType)
        {
            if (type.Namespace != genericType.NameSpace)
            {
                return false;
            }
            return type.Name == $"{genericType.Name}`{genericType.TypeParameterCount}";
        }
    }
}
