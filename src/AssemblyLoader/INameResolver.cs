using System;
using System.Reflection;
using System.Runtime.Serialization;
using AssemblyTypeLoader;
using Newtonsoft.Json;

namespace AssemblyLoader
{
    interface INameResolver
    {
        string GetName(MemberInfo info);
    }

    interface IPropertyNameResolver : INameResolver
    {
        string GetName(PropertyInfo propertyInfo);
    }

    public abstract class PropertyNameResolverBase : IPropertyNameResolver
    {
        string INameResolver.GetName(MemberInfo info)
        {
            return GetName(info as PropertyInfo);
        }

        public string GetName(PropertyInfo propertyInfo)
        {
            return GetName(propertyInfo, true);
        }

        public string GetName(PropertyInfo propertyInfo, bool throwOnError)
        {
            if (null == propertyInfo)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }
                return null;
            }

            return GetNameInner(propertyInfo, throwOnError);
        }

        abstract protected string GetNameInner(PropertyInfo propertyInfo, bool throwOnError);
    }

    public class PropertyNameResolver : PropertyNameResolverBase
    {
        protected override string GetNameInner(PropertyInfo propertyInfo, bool throwOnError)
        {
            DataMemberAttribute attr;
            if (propertyInfo.TryGetCustomAttribute(out attr))
            {
                if (!string.IsNullOrWhiteSpace(attr.Name))
                {
                    return attr.Name;
                }
            }
            return propertyInfo.Name;
        }
    }

    public class NewtonsoftJsonNameResolve : PropertyNameResolverBase
    {
        protected override string GetNameInner(PropertyInfo propertyInfo, bool throwOnError)
        {
           JsonPropertyAttribute attr;
            if (propertyInfo.TryGetCustomAttribute(out attr))
            {
                if (!string.IsNullOrWhiteSpace(attr.PropertyName))
                {
                    return attr.PropertyName;
                }
            }
            return propertyInfo.Name;
        }
    }

    public class CamelCaseNameResolve : PropertyNameResolverBase
    {
        protected override string GetNameInner(PropertyInfo propertyInfo, bool throwOnError)
        {
            return char.IsUpper(propertyInfo.Name[0])
                            ? $"{char.ToLower(propertyInfo.Name[0])}{propertyInfo.Name.Substring(1)}"
                            : propertyInfo.Name;
        }
    }

    public class PassthruNameResolver : PropertyNameResolverBase
    {
        protected override string GetNameInner(PropertyInfo propertyInfo, bool throwOnError)
        {
            return propertyInfo.Name;
        }
    }
}
