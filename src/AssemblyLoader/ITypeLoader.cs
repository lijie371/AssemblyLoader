using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyTypeLoader
{
    public interface ITypeLoader
    {
        Type[] FetchTypes(Assembly assembly, params ITypeSelector[] selectors);
    }

    public class TypeLoader : ITypeLoader
    {
        protected readonly IList<ITypeSelector> TypeSelectors;

        public TypeLoader(IEnumerable<ITypeSelector> typeSelectors = null)
        {
            this.TypeSelectors = new List<ITypeSelector>(typeSelectors ?? Enumerable.Empty<ITypeSelector>());
        }
         
        public Type[] FetchTypes(Assembly assembly, params ITypeSelector[] selectors)
        {
            return this.FetchTypes(assembly)
                       .Where(x => selectors.Any(y => y.ShouldKeepType(x)))
                       .ToArray();
        }

        protected virtual IEnumerable<Type> FetchTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                           .Where(x => !x.GetTypeInfo().HasCustomAttribute<CompilerGeneratedAttribute>())
                           .Where(x => this.TypeSelectors.Count == 0 || this.TypeSelectors.Any(y => y.ShouldKeepType(x)));
        }
    }

    public class DtoTypeLoader : TypeLoader
    {
        public DtoTypeLoader() : base(new ITypeSelector[] { new ClassTypeSelector(), new EnumTypeSelector(), new ValueTypeSelector() }) { }
    }

    public class ServiceTypeLoader : TypeLoader
    {
        public ServiceTypeLoader(): base(new ITypeSelector[] { new InterfaceTypeSelector() }) { }
    }

    public class ServiceWalkerTypeLoader : TypeLoader
    {
        protected override IEnumerable<Type> FetchTypes(Assembly assembly)
        {
            HashSet<Type> exportTypes = new HashSet<Type>();
            var interfaceTypes = base.FetchTypes(assembly).Where(x => x.GetTypeInfo().IsInterface && x.GetRuntimeMethods().Any(y => !y.IsSpecialName));
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