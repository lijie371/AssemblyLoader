using System;
using System.Reflection;

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
    }
}