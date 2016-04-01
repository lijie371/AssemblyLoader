using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyTypeLoader
{
    public interface ITypeLoader
    {
        Type[] FetchTypes(Assembly assembly);
    }

    public class TypeLoader : ITypeLoader
    {
        protected readonly IList<ITypeSelector> TypeSelectors;

        public TypeLoader(IEnumerable<ITypeSelector> typeSelectors = null)
        {
            this.TypeSelectors = new List<ITypeSelector>(typeSelectors ?? Enumerable.Empty<ITypeSelector>());
        }

        public virtual Type[] FetchTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                           .RemoveClosureTypes()
                           .Where(x => this.TypeSelectors.Count == 0 || this.TypeSelectors.Any(y => y.ShouldKeepType(x)))
                           .ToArray();
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
        public ServiceWalkerTypeLoader() : base(new ITypeSelector[] { new InterfaceTypeSelector() }) { }

        public override Type[] FetchTypes(Assembly assembly)
        {
            HashSet<Type> exportTypes = new HashSet<Type>();
            foreach (var t in base.FetchTypes(assembly).Where(x => x.GetRuntimeMethods().Any(y => !y.IsSpecialName)))
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
            return exportTypes.ToArray();
        }
    }
}