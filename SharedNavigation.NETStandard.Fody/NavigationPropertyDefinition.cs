using Mono.Cecil;

namespace SharedNavigation.NETStandard.Fody
{
    public class NavigationPropertyDefinition
    {
        public MethodDefinition GetNavigationCommandMethod { get; }

        public MethodReference SetNavigationActionMethod { get; }

        public NavigationPropertyDefinition(
            MethodDefinition getNavigationCommandMethod,
            MethodReference setNavigationActionMethod)
        {
            GetNavigationCommandMethod = getNavigationCommandMethod;
            SetNavigationActionMethod = setNavigationActionMethod;
        }
    }
}
