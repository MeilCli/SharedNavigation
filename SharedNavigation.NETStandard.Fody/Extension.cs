using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace SharedNavigation.NETStandard.Fody
{
    internal static class Extension
    {
        public static bool MatchType(this TypeReference type, Constant.TypeSpec typeSpec)
        {
            return type.FullName == typeSpec.FullName;
        }

        public static bool MatchGenericType(this TypeReference type, Constant.GenericTypeSpec genericTypeSpec)
        {
            if (type.Namespace != genericTypeSpec.NameSpace)
            {
                return false;
            }
            return type.Name == $"{genericTypeSpec.Name}`{genericTypeSpec.TypeParameterCount}";
        }

        public static bool MatchAttribute(this TypeReference type, Constant.AttributeSpec attributeSpec)
        {
            return type.FullName == attributeSpec.FullName;
        }

        public static bool MatchMethod(this MethodReference method, Constant.MethodSpec methodSpec)
        {
            return method.Name == methodSpec.Name;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                hashSet.Add(element);
            }
        }
    }
}
