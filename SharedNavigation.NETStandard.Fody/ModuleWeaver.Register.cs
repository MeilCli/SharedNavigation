using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedNavigation.NETStandard.Fody
{
    public partial class ModuleWeaver
    {
        private void applyRegisterNavigation(
            TypeDefinition navigationView,
            MethodDefinition registerMethod,
            IList<(TypeDefinition navigationAction, NavigationPropertyDefinition property)> injectActions)
        {
            foreach ((TypeDefinition navigationAction, NavigationPropertyDefinition property) in injectActions)
            {
                var navigationActionConstructor = navigationAction.Methods.First(x => x.Name == ".ctor");

                var processor = registerMethod.Body.GetILProcessor();
                var start = registerMethod.Body.Instructions.First();
                processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_1));
                processor.InsertBefore(start, processor.Create(OpCodes.Callvirt, property.GetNavigationCommandMethod));
                processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(start, processor.Create(OpCodes.Newobj, navigationActionConstructor));
                processor.InsertBefore(start, processor.Create(OpCodes.Callvirt, property.SetNavigationActionMethod));
            }
        }
    }
}
