using Mono.Cecil;
using System.Collections.Generic;

namespace SharedNavigation.NETStandard.Fody
{
    public class NavigationViewDefinition
    {
        public TypeDefinition NavigationViewType;
        public IList<NavigationActionDefinition> NavigationActionDefinitions;

        public NavigationViewDefinition(
            TypeDefinition navigationViewType,
            IList<NavigationActionDefinition> navigationActionDefinitions)
        {
            NavigationViewType = navigationViewType;
            NavigationActionDefinitions = navigationActionDefinitions;
        }
    }
}
