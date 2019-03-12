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
            foreach (TypeDefinition navigationView in findNavigationView())
            {
                LogInfo("SNNF navigationView found");
                var navigateMethods = findNavigateMethod(navigationView).ToList();
                var canNavigateMethods = findCanNavigateMethod(navigationView).ToList();

                LogInfo($"SNNF navigateMethodCount: {navigateMethods.Count}");

                var navigationActions = navigateMethods
                    .Select((x, i) => (type: createNavigationAction(navigationView, x.method, null, i), name: x.injectPropertyName))
                    .ToList();

                applyRegisterNavigation(navigationView, navigationActions);
            }
        }
    }
}
