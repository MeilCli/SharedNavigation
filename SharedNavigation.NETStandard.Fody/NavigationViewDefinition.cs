using Mono.Cecil;
using System.Collections.Generic;

namespace SharedNavigation.NETStandard.Fody
{
    public class NavigationViewDefinition
    {
        public TypeDefinition NavigationViewType { get; }
        public IList<NavigationActionDefinition> NavigationActionDefinitions { get; }
        public MethodDefinition RegisterNavigationMethod { get; }

        public NavigationViewDefinition(
            TypeDefinition navigationViewType,
            IList<NavigationActionDefinition> navigationActionDefinitions,
            MethodDefinition registerNavigationMethod)
        {
            NavigationViewType = navigationViewType;
            NavigationActionDefinitions = navigationActionDefinitions;
            RegisterNavigationMethod = registerNavigationMethod;
        }
    }
}
