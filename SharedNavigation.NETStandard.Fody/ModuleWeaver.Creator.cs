using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedNavigation.NETStandard.Fody
{
    public partial class ModuleWeaver
    {
        private TypeDefinition createNavigationAction(
            TypeDefinition navigationView,
            MethodDefinition navigateMethod,
            MethodDefinition canNavigateMethod,
            int uniqueId)
        {
            var navigationAction = new TypeDefinition(
                navigationView.Namespace,
                $"<N>NavigationAction{uniqueId}",
                TypeAttributes.NestedPrivate | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                TypeSystem.ObjectReference
            );

            var navigationActionInterface = new InterfaceImplementation(
                ModuleDefinition.ImportReference(FindType(Constant.NavigationActionInterfaceFullName))
            );
            navigationAction.Interfaces.Add(navigationActionInterface);

            FieldDefinition viewField = addNavigationViewField(navigationAction, navigationView);
            addNavigationViewConstructor(navigationAction, navigationView, viewField);
            if (navigateMethod != null)
            {
                addNavigateAsyncIfDefined(navigationAction, navigateMethod, viewField);
            }
            else
            {
                addNavigateAsyncIfNotDefined(navigationAction);
            }
            if (canNavigateMethod != null)
            {
                addCanNavigateIfDefined(navigationAction, canNavigateMethod, viewField);
            }
            else
            {
                addCanNavigateIfNotDefined(navigationAction);
            }

            navigationView.NestedTypes.Add(navigationAction);
            return navigationAction;
        }

        private FieldDefinition addNavigationViewField(TypeDefinition navigationAction, TypeDefinition navigationView)
        {
            var field = new FieldDefinition(
                "view",
                FieldAttributes.Private | FieldAttributes.InitOnly,
                ModuleDefinition.ImportReference(navigationView)
            );
            navigationAction.Fields.Add(field);

            return field;
        }

        private void addNavigationViewConstructor(
            TypeDefinition navigationAction,
            TypeDefinition navigationView,
            FieldDefinition viewField)
        {
            var method = new MethodDefinition(
                ".ctor",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                TypeSystem.VoidReference
            );
            var parameter = new ParameterDefinition(ModuleDefinition.ImportReference(navigationView));

            method.Parameters.Add(parameter);

            MethodReference objectConstructor = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.GetConstructors().First());
            ILProcessor processor = method.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, objectConstructor);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Stfld, viewField);
            processor.Emit(OpCodes.Ret);

            navigationAction.Methods.Add(method);
        }

        private void addNavigateAsyncIfNotDefined(TypeDefinition navigationAction)
        {
            var method = new MethodDefinition(
                Constant.NavigationActionNavigateAsyncName,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                ModuleDefinition.ImportReference(FindType($"{Constant.SystemThreadingTasksNameSpace}.{nameof(Task)}"))
            );

            var genericParameter = new GenericParameter("T", method);
            genericParameter.HasDefaultConstructorConstraint = true;
            genericParameter.Constraints.Add(ModuleDefinition.ImportReference(FindType(Constant.NavigationViewModelInterfaceFullName)));
            method.GenericParameters.Add(genericParameter);

            var parameter = new ParameterDefinition(genericParameter);
            method.Parameters.Add(parameter);

            ILProcessor processor = method.Body.GetILProcessor();

            TypeReference taskReference = ModuleDefinition.ImportReference(FindType($"{Constant.SystemThreadingTasksNameSpace}.{nameof(Task)}"));
            processor.Append(processor.Create(OpCodes.Call, new MethodReference("get_CompletedTask", taskReference, taskReference)));
            processor.Append(processor.Create(OpCodes.Ret));

            navigationAction.Methods.Add(method);
        }

        private void addNavigateAsyncIfDefined(
            TypeDefinition navigationAction,
            MethodDefinition navigateMethod,
            FieldDefinition viewField)
        {
            var method = new MethodDefinition(
                Constant.NavigationActionNavigateAsyncName,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                ModuleDefinition.ImportReference(FindType($"{Constant.SystemThreadingTasksNameSpace}.{nameof(Task)}"))
            );

            var genericParameter = new GenericParameter("T", method);
            genericParameter.HasDefaultConstructorConstraint = true;
            genericParameter.Constraints.Add(ModuleDefinition.ImportReference(FindType(Constant.NavigationViewModelInterfaceFullName)));
            method.GenericParameters.Add(genericParameter);

            var parameter = new ParameterDefinition(genericParameter);
            method.Parameters.Add(parameter);

            method.Body.Variables.Add(new VariableDefinition(navigateMethod.Parameters[0].ParameterType));

            ILProcessor processor = method.Body.GetILProcessor();
            Instruction start = processor.Create(OpCodes.Nop);
            processor.Append(start);
            processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_1));
            processor.InsertBefore(start, processor.Create(OpCodes.Box, genericParameter));
            processor.InsertBefore(start, processor.Create(OpCodes.Isinst, navigateMethod.Parameters[0].ParameterType));
            processor.InsertBefore(start, processor.Create(OpCodes.Dup));
            processor.InsertBefore(start, processor.Create(OpCodes.Stloc_0));
            processor.InsertBefore(start, processor.Create(OpCodes.Brfalse_S, start));

            if (navigateMethod.IsStatic || navigateMethod.HasThis == false)
            {
                processor.InsertBefore(start, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(start, processor.Create(OpCodes.Call, navigateMethod));
            }
            else
            {
                processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(
                    start,
                    processor.Create(OpCodes.Ldfld, viewField)
                );
                processor.InsertBefore(start, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(start, processor.Create(OpCodes.Callvirt, navigateMethod));
            }

            processor.InsertBefore(start, processor.Create(OpCodes.Ret));

            TypeReference taskReference = ModuleDefinition.ImportReference(FindType($"{Constant.SystemThreadingTasksNameSpace}.{nameof(Task)}"));
            processor.Append(processor.Create(OpCodes.Call, new MethodReference("get_CompletedTask", taskReference, taskReference)));
            processor.Append(processor.Create(OpCodes.Ret));

            navigationAction.Methods.Add(method);
        }

        private void addCanNavigateIfNotDefined(TypeDefinition navigationAction)
        {
            var method = new MethodDefinition(
                Constant.NavigationActionCanNavigateName,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                ModuleDefinition.ImportReference(typeof(bool))
            );

            var genericParameter = new GenericParameter("T", method);
            genericParameter.HasDefaultConstructorConstraint = true;
            genericParameter.Constraints.Add(ModuleDefinition.ImportReference(FindType(Constant.NavigationViewModelInterfaceFullName)));
            method.GenericParameters.Add(genericParameter);

            var parameter = new ParameterDefinition(genericParameter);
            method.Parameters.Add(parameter);

            ILProcessor processor = method.Body.GetILProcessor();

            processor.Append(processor.Create(OpCodes.Ldc_I4_1));
            processor.Append(processor.Create(OpCodes.Ret));

            navigationAction.Methods.Add(method);
        }

        private void addCanNavigateIfDefined(
            TypeDefinition navigationAction,
            MethodDefinition canNavigateMethod,
            FieldDefinition viewField)
        {
            var method = new MethodDefinition(
                Constant.NavigationActionCanNavigateName,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                ModuleDefinition.ImportReference(typeof(bool))
            );

            var genericParameter = new GenericParameter("T", method);
            genericParameter.HasDefaultConstructorConstraint = true;
            genericParameter.Constraints.Add(ModuleDefinition.ImportReference(FindType(Constant.NavigationViewModelInterfaceFullName)));
            method.GenericParameters.Add(genericParameter);

            var parameter = new ParameterDefinition(genericParameter);
            method.Parameters.Add(parameter);

            method.Body.Variables.Add(new VariableDefinition(canNavigateMethod.Parameters[0].ParameterType));

            ILProcessor processor = method.Body.GetILProcessor();
            Instruction start = processor.Create(OpCodes.Nop);
            processor.Append(start);
            processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_1));
            processor.InsertBefore(start, processor.Create(OpCodes.Box, genericParameter));
            processor.InsertBefore(start, processor.Create(OpCodes.Isinst, canNavigateMethod.Parameters[0].ParameterType));
            processor.InsertBefore(start, processor.Create(OpCodes.Dup));
            processor.InsertBefore(start, processor.Create(OpCodes.Stloc_0));
            processor.InsertBefore(start, processor.Create(OpCodes.Brfalse_S, start));
            if (canNavigateMethod.IsStatic || canNavigateMethod.HasThis == false)
            {
                processor.InsertBefore(start, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(start, processor.Create(OpCodes.Call, canNavigateMethod));
            }
            else
            {
                processor.InsertBefore(start, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(
                    start,
                    processor.Create(OpCodes.Ldfld, viewField)
                );
                processor.InsertBefore(start, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(start, processor.Create(OpCodes.Callvirt, canNavigateMethod));
            }
            processor.InsertBefore(start, processor.Create(OpCodes.Ret));

            processor.Append(processor.Create(OpCodes.Ldc_I4_1));
            processor.Append(processor.Create(OpCodes.Ret));

            navigationAction.Methods.Add(method);
        }
    }
}
