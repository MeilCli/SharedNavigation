using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedNavigation.NETStandard.Fody
{
    internal static class Constant
    {
        public const string Tag = "SNNF";

        internal const string TargetNameSpace = "SharedNavigation.NETStandard";
        internal const string SystemThreadingTasksNameSpace = "System.Threading.Tasks";

        public static readonly TypeSpec Task = new TypeSpec(typeof(Task));

        internal const string NavigationActionInterfaceFullName = "SharedNavigation.NETStandard.INavigationAction";
        internal const string NavigationActionNavigateAsyncName = "NavigateAsync";
        internal const string NavigationActionCanNavigateName = "CanNavigate";

        internal const string NavigationViewInterfaceFullName = "SharedNavigation.NETStandard.INavigationView";
        public static readonly GenericTypeSpec NavigationView = new GenericTypeSpec(
            "SharedNavigation.NETStandard", "INavigationView", 1
        );
        internal const string NavigationViewRegisterNavigationByAutoGeneratedName =
            "RegisterNavigationByAutoGenerated";

        internal const string NavigationViewModelInterfaceFullName = "SharedNavigation.NETStandard.INavigationViewModel";

        internal static readonly GenericTypeSpec NavigationCommand = new GenericTypeSpec(
            "SharedNavigation.NETStandard", "INavigationCommand", 1
        );
        internal const string NavigationCommandNavigationActionName = "NavigationAction";

        public static readonly AttributeSpec NavigateAttribute = new AttributeSpec(
            "SharedNavigation.NETStandard", "NavigateAttribute"
        );
        public static readonly AttributeSpec CanNavigateAttribute = new AttributeSpec(
            "SharedNavigation.NETStandard", "CanNavigateAttribute"
        );

        public class TypeSpec
        {
            public string NameSpace { get; }
            public string Name { get; }
            public string FullName => $"{NameSpace}.{Name}";

            public TypeSpec(Type type) : this(type.Namespace, type.Name) { }

            public TypeSpec(string nameSpace, string name)
            {
                NameSpace = nameSpace;
                Name = name;
            }
        }

        public class GenericTypeSpec
        {
            public string NameSpace { get; }
            public string Name { get; }
            public string FullName => $"{NameSpace}.{Name}";
            public int TypeParameterCount { get; }

            public GenericTypeSpec(string nameSpace, string name, int typeParameterCount)
            {
                NameSpace = nameSpace;
                Name = name;
                TypeParameterCount = typeParameterCount;
            }
        }

        public class AttributeSpec
        {
            public string NameSpace { get; }
            public string Name { get; }
            public string FullName => $"{NameSpace}.{Name}";

            public AttributeSpec(string nameSpace, string name)
            {
                NameSpace = nameSpace;
                Name = name;
            }
        }
    }
}
