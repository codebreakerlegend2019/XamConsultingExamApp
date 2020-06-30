﻿using DryIoc;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace XamConsultingTest.Helpers
{
    public static class ContainerRegistryExtension
    {
        public static void AutoRegisterByNameSpace(this IContainerRegistry containerRegistry, string name)
        {
            var classes = GetRespositoryPatternClassesByNameSpace(name);
            foreach (var classs in classes)
                containerRegistry
                    .Register
                    (classs
                    .GetInterfaces()
                    .FirstOrDefault
                    (x => x.Name.Contains($"I{classs.Name}"))
                    , classs);
        }
        public static void AutoRegisterByInterfaceName(this IContainerRegistry containerRegistry, string interfaceName)
        {
            try
            {

                var classes = GetClasseseOfAnInterface(interfaceName);
                foreach (var classs in classes)
                    foreach (var iInterface in classs.GetInterfaces()
                        .Where(x => x.Name.Contains(interfaceName) && x.Namespace.Contains(nameof(XamConsultingTest)))
                        .ToList())
                        containerRegistry.Register(iInterface, classs);
            }
            catch(Exception ex)
            {

            }
        }
        private static List<Type> GetRespositoryPatternClassesByNameSpace(string name)
        {
            var filteredTypes = new List<Type>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass).OrderBy(x => x.Name).ToList();

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                var isRepositoryPatternVerified = (interfaces.FirstOrDefault(x => x.Name.Contains($"I{type.Name}"))) != null;
                if (isRepositoryPatternVerified)
                    filteredTypes.Add(type);
            }
            return filteredTypes.Where(x => x.Namespace.Contains(name)).ToList();
        }
        private static List<Type> GetClasseseOfAnInterface(string interfaceName)
        {
            return (AppDomain.CurrentDomain
                     .GetAssemblies()
                     .SelectMany(x => x.GetTypes())
                     .Where(x => x.IsClass)
                     .OrderBy(x => x.Name))
                     .Where(x => x.GetInterfaces().Any(xx => xx.Name.Contains(interfaceName) && xx.Namespace.Contains(nameof(XamConsultingTest))))
                     .ToList();

        }
    }
}
