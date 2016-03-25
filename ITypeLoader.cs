using System;
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
            return assembly.GetTypes()
                           // .Where(x => (type.IsClass || (type.IsValueType && !type.IsPrimitive && !type.IsEnum) || type.IsEnum /**/)
                           .Where(x => selectors.Any(y => y.ShouldKeepType(x)))
                           .ToArray();
        }
    }
}
