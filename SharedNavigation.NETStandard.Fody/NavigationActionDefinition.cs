using Mono.Cecil;

namespace SharedNavigation.NETStandard.Fody
{
    public class NavigationActionDefinition
    {
        public string InjectPropertyName { get; }
        public MethodDefinition NavigateMethod { get; }
        public MethodDefinition CanNavigateMethod { get; }

        public NavigationActionDefinition(
            string injectPropertyName,
            MethodDefinition navigateMethod,
            MethodDefinition canNavigateMethod)
        {
            InjectPropertyName = injectPropertyName;
            NavigateMethod = navigateMethod;
            CanNavigateMethod = canNavigateMethod;
        }
    }
}
