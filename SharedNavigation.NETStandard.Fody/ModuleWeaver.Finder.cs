using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedNavigation.NETStandard.Fody
{
    public partial class ModuleWeaver
    {
        private IEnumerable<TypeDefinition> findNavigationView()
        {
            LogInfo("SNNF start findNavivationView");
            foreach (TypeDefinition type in ModuleDefinition.Types)
            {
                LogInfo($"SNNF type: {type.FullName}");
                if (type.IsInterface)
                {
                    LogInfo("SNNF isInteface");
                    continue;
                }
                if (type.HasInterfaces == false)
                {
                    LogInfo("SNNF has not interfaces");
                    continue;
                }
                if (type.HasMethods == false)
                {
                    LogInfo("SNNF has not methods");
                    continue;
                }

                if (isImplementedNavigationViewInterface(type) == false)
                {
                    LogInfo("SNNF is not interface");
                    continue;
                }

                yield return type;
            }
        }

        private bool isImplementedNavigationViewInterface(TypeDefinition type)
        {
            LogInfo("SNNF interface scan");
            foreach (InterfaceImplementation interfaceImplementation in type.Interfaces)
            {
                if (interfaceImplementation.InterfaceType.EqualGenericType(Constant.NavigationView))
                {
                    LogInfo("SNNF Match");
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<(MethodDefinition method, string injectPropertyName)> findNavigateMethod(TypeDefinition type)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (tryGetInjectPropertyNameInNavigateAttribute(method, out string injectPropertyName) == false)
                {
                    continue;
                }

                if (validNavigateMethod(method) == false)
                {
                    continue;
                }

                yield return (method, injectPropertyName);
            }
        }

        private bool tryGetInjectPropertyNameInNavigateAttribute(MethodDefinition method, out string injectPropertyName)
        {
            foreach (CustomAttribute customAttribute in method.CustomAttributes)
            {
                if (customAttribute.AttributeType.FullName == Constant.NavigateAttributeFullName)
                {
                    if (customAttribute.ConstructorArguments.Count == 0)
                    {
                        LogError($"{Constant.NavigateAttributeFullName} has not constructor argument");
                    }
                    if (customAttribute.ConstructorArguments[0].Value == null)
                    {
                        LogError($"{Constant.NavigateAttributeFullName} has null constructor argument");
                    }

                    injectPropertyName = customAttribute.ConstructorArguments[0].Value.ToString();
                    return true;
                }
            }

            injectPropertyName = default;
            return false;
        }

        private bool validNavigateMethod(MethodDefinition method)
        {
            if (method.HasParameters == false)
            {
                LogWarning($"{Constant.NavigateAttributeFullName} method must have 1 parameter");
                return false;
            }
            if (method.Parameters.Count != 1)
            {
                LogWarning($"{Constant.NavigateAttributeFullName} method must have 1 parameter");
                return false;
            }

            if (method.ReturnType.EqualType(typeof(Task)) == false
                || method.ReturnType.HasGenericParameters)
            {
                LogWarning($"{Constant.NavigateAttributeFullName} method must return Task");
                return false;
            }

            // not look parameter type, because it checked when runtime.

            return true;
        }

        private IEnumerable<(MethodDefinition method, string injectPropertyName)> findCanNavigateMethod(TypeDefinition type)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (tryGetInjectPropertyNameInCanNavigateAttribute(method, out string injectPropertyName) == false)
                {
                    continue;
                }

                if (validCanNavigateMethod(method) == false)
                {
                    continue;
                }

                yield return (method, injectPropertyName);
            }
        }

        private bool tryGetInjectPropertyNameInCanNavigateAttribute(MethodDefinition method, out string injectPropertyName)
        {
            foreach (CustomAttribute customAttribute in method.CustomAttributes)
            {
                if (customAttribute.AttributeType.FullName == Constant.CanNavigateAttributeFullName)
                {
                    if (customAttribute.ConstructorArguments.Count == 0)
                    {
                        LogError($"{Constant.CanNavigateAttributeFullName} has not constructor argument");
                    }
                    if (customAttribute.ConstructorArguments[0].Value == null)
                    {
                        LogError($"{Constant.CanNavigateAttributeFullName} has null constructor argument");
                    }

                    injectPropertyName = customAttribute.ConstructorArguments[0].Value.ToString();
                    return true;
                }
            }

            injectPropertyName = default;
            return false;
        }

        private bool validCanNavigateMethod(MethodDefinition method)
        {
            if (method.HasParameters == false)
            {
                LogWarning($"{Constant.CanNavigateAttributeFullName} method must have 1 parameter");
                return false;
            }
            if (method.Parameters.Count != 1)
            {
                LogWarning($"{Constant.CanNavigateAttributeFullName} method must have 1 parameter");
                return false;
            }

            if (method.ReturnType.MetadataType != MetadataType.Boolean)
            {
                LogWarning($"{Constant.CanNavigateAttributeFullName} method must return bool");
                return false;
            }

            // not look parameter type, because it checked when runtime.

            return true;
        }
    }
}
