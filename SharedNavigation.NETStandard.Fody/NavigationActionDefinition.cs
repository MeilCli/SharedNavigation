using Mono.Cecil;

namespace SharedNavigation.NETStandard.Fody
{
    public class NavigationActionDefinition
    {
        public string InjectPropertyName { get; }
        public NavigationPropertyDefinition InjectProperty { get; }

        public MethodDefinition NavigateMethod { get; }
        public MethodDefinition CanNavigateMethod { get; }

        public NavigationActionDefinition(
            string injectPropertyName,
            NavigationPropertyDefinition injectProperty,
            MethodDefinition navigateMethod,
            MethodDefinition canNavigateMethod)
        {
            InjectPropertyName = injectPropertyName;
            InjectProperty = injectProperty;
            NavigateMethod = navigateMethod;
            CanNavigateMethod = canNavigateMethod;
        }
    }
}
