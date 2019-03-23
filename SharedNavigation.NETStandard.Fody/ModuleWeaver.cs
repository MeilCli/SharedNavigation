using Fody;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharedNavigation.NETStandard.Fody
{
    public partial class ModuleWeaver : BaseModuleWeaver
    {
        public override IEnumerable<string> GetAssembliesForScanning()
        {
            var assemblies = new List<string>();

            assemblies.Add("netstandard");
            assemblies.Add("mscorlib");
            assemblies.Add("SharedNavigation.NETStandard");

            return assemblies;
        }

        public override void Execute()
        {
            var navigationFinder = new NavigationFinder(this);

            foreach (var navigationViewDefinition in navigationFinder.FindNavigationViewDefinitions())
            {
                logNavigationViewDefinition(navigationViewDefinition);

                var navigationActions = navigationViewDefinition
                    .NavigationActionDefinitions
                    .Select((x, i) => (
                        type: createNavigationAction(
                            navigationViewDefinition.NavigationViewType,
                            x.NavigateMethod,
                            x.CanNavigateMethod,
                            i),
                        property: x.InjectProperty)
                    )
                    .ToList();

                applyRegisterNavigation(
                    navigationViewDefinition.NavigationViewType,
                    navigationViewDefinition.RegisterNavigationMethod,
                    navigationActions
                );
            }
        }

        private void logNavigationViewDefinition(NavigationViewDefinition navigationViewDefinition)
        {
            LogInfo($"{Constant.Tag}: navigation view: {navigationViewDefinition.NavigationViewType.FullName}");

            foreach (var navigationActionDefinition in navigationViewDefinition.NavigationActionDefinitions)
            {
                LogInfo($"{Constant.Tag}: navigation action: property: {navigationActionDefinition.InjectPropertyName}, " +
                    $"navigate: {navigationActionDefinition.NavigateMethod?.Name ?? "undefined"}, " +
                    $"can navigate: {navigationActionDefinition.CanNavigateMethod?.Name ?? "undefined"}");
            }
        }
    }
}
