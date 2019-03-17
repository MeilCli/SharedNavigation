using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace SharedNavigation.NETStandard.Fody
{
    public class NavigationFinder
    {
        private readonly ModuleWeaver moduleWeaver;

        public NavigationFinder(ModuleWeaver moduleWeaver)
        {
            this.moduleWeaver = moduleWeaver;
        }

        private void logInfo(TypeDefinition typeDefinition, string message)
        {
            moduleWeaver.LogInfo($"{Constant.Tag}: {typeDefinition.FullName}: {message}");
        }

        private void logWarning(TypeDefinition typeDefinition, string message)
        {
            moduleWeaver.LogWarning($"{Constant.Tag}: {typeDefinition.FullName}: {message}");
        }

        private void logError(TypeDefinition typeDefinition, string message)
        {
            moduleWeaver.LogError($"{Constant.Tag}: {typeDefinition.FullName}: {message}");
        }

        public IEnumerable<NavigationViewDefinition> FindNavigationViewDefinitions()
        {
            foreach (TypeDefinition type in moduleWeaver.ModuleDefinition.Types)
            {
                if (isNavigationView(type) == false)
                {
                    continue;
                }

                var navigateMethods = findNavigateMethods(type).ToList();
                var canNavigateMethods = findCanNavigateMethods(type).ToList();

                if (navigateMethods.Select(x => x.injectPropertyName).Distinct().Count() != navigateMethods.Count)
                {
                    logWarning(type, $"{Constant.NavigateAttribute.Name} value is conflicted");
                }
                if (canNavigateMethods.Select(x => x.injectPropertyName).Distinct().Count() != canNavigateMethods.Count)
                {
                    logWarning(type, $"{Constant.CanNavigateAttribute.Name} value is conflicted");
                }

                var injectPropertyNames = new HashSet<string>();
                injectPropertyNames.AddRange(navigateMethods.Select(x => x.injectPropertyName));
                injectPropertyNames.AddRange(canNavigateMethods.Select(x => x.injectPropertyName));

                IList<NavigationActionDefinition> navigationActionDefinitions = injectPropertyNames
                    .Select(x =>
                    {
                        MethodDefinition navigateMethod = navigateMethods
                            .Where(y => y.injectPropertyName == x)
                            .Select(y => y.method)
                            .FirstOrDefault();
                        MethodDefinition canNavigateMethod = canNavigateMethods
                            .Where(y => y.injectPropertyName == x)
                            .Select(y => y.method)
                            .FirstOrDefault();
                        return new NavigationActionDefinition(x, navigateMethod, canNavigateMethod);
                    })
                    .ToList();

                yield return new NavigationViewDefinition(type, navigationActionDefinitions);
            }
        }

        private bool isNavigationView(TypeDefinition type)
        {
            if (type.IsInterface)
            {
                logInfo(type, "is interface");
                return false;
            }
            if (type.HasInterfaces == false)
            {
                logInfo(type, "has not interface");
                return false;
            }
            if (type.HasMethods == false)
            {
                logInfo(type, "has not method");
                return false;
            }

            if (isImplementedNavigationViewInterface(type) == false)
            {
                logInfo(type, $"not implement {Constant.NavigationView.Name}");
                return false;
            }

            return true;
        }

        private bool isImplementedNavigationViewInterface(TypeDefinition type)
        {
            foreach (InterfaceImplementation interfaceImplementation in type.Interfaces)
            {
                if (interfaceImplementation.InterfaceType.MatchGenericType(Constant.NavigationView))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<(MethodDefinition method, string injectPropertyName)> findNavigateMethods(
            TypeDefinition navigationView)
        {
            foreach (MethodDefinition method in navigationView.Methods)
            {
                if (tryGetInjectPropertyNameInNavigateAttribute(navigationView, method, out string injectPropertyName) == false)
                {
                    logInfo(navigationView, $"{method.Name}: has not {Constant.NavigateAttribute.Name}");
                    continue;
                }

                if (validNavigateMethod(navigationView, method) == false)
                {
                    continue;
                }

                yield return (method, injectPropertyName);
            }
        }

        private bool tryGetInjectPropertyNameInNavigateAttribute(
            TypeDefinition navigationView,
            MethodDefinition method,
            out string injectPropertyName)
        {
            foreach (CustomAttribute customAttribute in method.CustomAttributes)
            {
                if (customAttribute.AttributeType.MatchAttribute(Constant.NavigateAttribute))
                {
                    if (customAttribute.ConstructorArguments.Count != 1 ||
                        customAttribute.ConstructorArguments[0].Value == null)
                    {
                        logError(
                            navigationView,
                            $"{method.Name}: {Constant.NavigateAttribute.Name} has not constructor argument"
                        );
                        continue;
                    }

                    injectPropertyName = customAttribute.ConstructorArguments[0].Value.ToString();
                    return true;
                }
            }

            injectPropertyName = default;
            return false;
        }

        private bool validNavigateMethod(TypeDefinition navigationView, MethodDefinition method)
        {
            if (method.HasParameters == false || method.Parameters.Count != 1)
            {
                logWarning(
                    navigationView,
                    $"{method.Name}: {Constant.NavigateAttribute.Name} method must have 1 parameter"
                );
                return false;
            }

            if (method.ReturnType.MatchType(Constant.Task) == false ||
                method.ReturnType.HasGenericParameters)
            {
                logWarning(
                    navigationView,
                    $"{method.Name}: {Constant.NavigateAttribute.Name} method must return Task"
                );
                return false;
            }

            // not look parameter type, because it checked when runtime.

            return true;
        }

        private IEnumerable<(MethodDefinition method, string injectPropertyName)> findCanNavigateMethods(
            TypeDefinition navigationView)
        {
            foreach (MethodDefinition method in navigationView.Methods)
            {
                if (tryGetInjectPropertyNameInCanNavigateAttribute(navigationView, method, out string injectPropertyName) == false)
                {
                    logInfo(navigationView, $"{method.Name}: has not {Constant.CanNavigateAttribute.Name}");
                    continue;
                }

                if (validCanNavigateMethod(navigationView, method) == false)
                {
                    continue;
                }

                yield return (method, injectPropertyName);
            }
        }

        private bool tryGetInjectPropertyNameInCanNavigateAttribute(
            TypeDefinition navigationView,
            MethodDefinition method,
            out string injectPropertyName)
        {
            foreach (CustomAttribute customAttribute in method.CustomAttributes)
            {
                if (customAttribute.AttributeType.MatchAttribute(Constant.CanNavigateAttribute))
                {
                    if (customAttribute.ConstructorArguments.Count != 1 ||
                        customAttribute.ConstructorArguments[0].Value == null)
                    {
                        logError(
                            navigationView,
                            $"{method.Name}: {Constant.NavigateAttribute.Name} has not constructor argument"
                        );
                        continue;
                    }

                    injectPropertyName = customAttribute.ConstructorArguments[0].Value.ToString();
                    return true;
                }
            }

            injectPropertyName = default;
            return false;
        }

        private bool validCanNavigateMethod(TypeDefinition navigationView, MethodDefinition method)
        {
            if (method.HasParameters == false || method.Parameters.Count != 1)
            {
                logWarning(
                    navigationView,
                    $"{method.Name}: {Constant.CanNavigateAttribute.Name} method must have 1 parameter"
                );
                return false;
            }

            if (method.ReturnType.MetadataType != MetadataType.Boolean)
            {
                logWarning(
                    navigationView,
                    $"{method.Name}: {Constant.CanNavigateAttribute.Name} method must return bool"
                );
                return false;
            }

            // not look parameter type, because it checked when runtime.

            return true;
        }
    }
}
