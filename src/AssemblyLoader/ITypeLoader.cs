using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyTypeLoader
{
    public interface ITypeLoader
    {
        Type[] FetchTypes(Assembly assembly, params ITypeSelector[] selectors);
    }

    public class TypeLoader : ITypeLoader
    {
        public Type[] FetchTypes(Assembly assembly, params ITypeSelector[] selectors)
        {
            return this.FetchTypes(assembly)
                       .Where(x => selectors.Any(y => y.ShouldKeepType(x)))
                       .ToArray();
        }

        protected virtual IEnumerable<Type> FetchTypes(Assembly assembly)
        {
            return assembly.GetTypes();
        }
    }

    public class DtoTypeLoader : TypeLoader
    {
        protected override IEnumerable<Type> FetchTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(x => x.GetTypeInfo().IsClass || x.GetTypeInfo().IsEnum || (x.GetTypeInfo().IsValueType && !x.GetTypeInfo().IsPrimitive));
        }
    }

    public class ServiceTypeLoader : TypeLoader
    {
        protected override IEnumerable<Type> FetchTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(x => x.GetTypeInfo().IsInterface);
        }
    }

    public class ServiceWalkerTypeLoader : TypeLoader
    {
        protected override IEnumerable<Type> FetchTypes(Assembly assembly)
        {
            HashSet<Type> exportTypes = new HashSet<Type>();
            var interfaceTypes = assembly.GetTypes().Where(x => x.GetTypeInfo().IsInterface && x.GetRuntimeMethods().Any(y => !y.IsSpecialName));
            foreach (var t in interfaceTypes)
            {
                exportTypes.Add(t);
                foreach (var method in t.GetRuntimeMethods().Where(y => !y.IsSpecialName))
                {
                    exportTypes.Add(method.ReturnType);
                    foreach (var methodInputType in method.GetParameters().Select(x => x.ParameterType))
                    {
                        exportTypes.Add(methodInputType);
                    }
                }
            }
            return exportTypes;
        }
    }
}