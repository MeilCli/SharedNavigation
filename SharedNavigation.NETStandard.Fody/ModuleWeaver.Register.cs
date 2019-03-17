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
            IList<(TypeDefinition navigationAction, string name)> injectActions)
        {
            MethodDefinition registerMethod = findRegisterNavigation(navigationView);
            if (registerMethod is null)
            {
                LogWarning($"SNNF {Constant.NavigationViewRegisterNavigationByAutoGeneratedName} was not found");
            }

            // Register method first parameter is ViewModel
            TypeDefinition viewModel = registerMethod.Parameters[0].ParameterType.Resolve();

            foreach ((TypeDefinition navigationAction, string name) in injectActions)
            {
                PropertyDefinition injectProperty = viewModel.Properties.FirstOrDefault(x => x.Name == name);
                if (injectProperty is null)
                {
                    LogWarning($"SNNF {name} was not found");
                    continue;
                }
                if (injectProperty.PropertyType.MatchGenericType(Constant.NavigationCommand) == false)
                {
                    LogWarning($"SNNF {name} is not {Constant.NavigationCommand.FullName}");
                }

                var navigationCommandType = ModuleDefinition.ImportReference(injectProperty.PropertyType);
                var navigationActionInterface = ModuleDefinition.ImportReference(FindType(Constant.NavigationActionInterfaceFullName));
                var navigationActionPropertySetter = new MethodReference(
                    "set_" + Constant.NavigationCommandNavigationActionName,
                    TypeSystem.VoidReference,
                    navigationCommandType
                );
                navigationActionPropertySetter.HasThis = true;
                navigationActionPropertySetter.Parameters.Add(new ParameterDefinition(navigationActionInterface));
                var navigationActionConstructor = navigationAction.Methods.First(x => x.Name == ".ctor");

                var processor = registerMethod.Body.GetILProcessor();
                var start = registerMethod.Body.Instructions.First();
                processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_1));
                processor.InsertBefore(start, processor.Create(OpCodes.Callvirt, injectProperty.GetMethod));
                processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(start, processor.Create(OpCodes.Newobj, navigationActionConstructor));
                processor.InsertBefore(start, processor.Create(OpCodes.Callvirt, navigationActionPropertySetter));
            }
        }

        private MethodDefinition findRegisterNavigation(TypeDefinition navigationView)
        {
            return navigationView
                .Methods
                // インターフェースメソッドの明示的な実装がされている場合は.overrideディレクティブが宣言されている
                .Where(x => x.Overrides.Any())
                // どの型に対して明示的な実装をしているかの確認
                .Where(x => x.Overrides.First().DeclaringType.MatchGenericType(Constant.NavigationView))
                .Where(x => x.Overrides.First().Name == Constant.NavigationViewRegisterNavigationByAutoGeneratedName)
                .FirstOrDefault() ??
                navigationView
                .Methods
                .FirstOrDefault(x => x.Name == Constant.NavigationViewRegisterNavigationByAutoGeneratedName);
        }
    }
}
