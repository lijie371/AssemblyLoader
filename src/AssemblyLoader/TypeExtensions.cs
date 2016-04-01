using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyTypeLoader
{
    public static class TypeExtensions
    {
        public static bool HasCustomAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            T attr;
            return TryGetCustomAttribute(memberInfo, out attr);
        }

        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, out T attribute) where T : Attribute
        {
            attribute = memberInfo.GetCustomAttribute<T>();
            return attribute != null;
        }

        public static IEnumerable<Type> RemoveClosureTypes(this IEnumerable<Type> types)
        {
            return types.Where(x => !x.GetTypeInfo().HasCustomAttribute<CompilerGeneratedAttribute>());
        }
    }
}