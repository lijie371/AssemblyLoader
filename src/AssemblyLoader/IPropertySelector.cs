using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AssemblyTypeLoader
{
    public enum ePropertyConversionType
    {
        None = 0,
        Optional = 1,
        Required = 2
    }

    public interface IPropertySelector
    {
        ePropertyConversionType GetPropertyConversionType(PropertyInfo propertyInfo);
    }

    public class DataMemberPropertySelector : IPropertySelector
    {
        public ePropertyConversionType GetPropertyConversionType(PropertyInfo propertyInfo)
        {
            DataMemberAttribute attr;
            if (!propertyInfo.TryGetCustomAttribute(out attr))
            {
                return ePropertyConversionType.None;
            }

            return attr.IsRequired ? ePropertyConversionType.Required : ePropertyConversionType.Optional;
        }
    }

    public class NewtonsoftJsonPropertySelector : IPropertySelector
    {
        public ePropertyConversionType GetPropertyConversionType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.HasCustomAttribute<JsonIgnoreAttribute>())
            {
                return ePropertyConversionType.None;
            }

            JsonPropertyAttribute attr;
            if (!propertyInfo.TryGetCustomAttribute(out attr))
            {
                return ePropertyConversionType.None;
            }

            if (attr.Required == Required.Always || attr.Required == Required.AllowNull)
            {
                return ePropertyConversionType.Required;
            }

            return ePropertyConversionType.Optional;
        }
    }

#if NET451
	public class SerializablePropertySelector : IPropertySelector
    {
        public ePropertyConversionType GetPropertyConversionType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.HasCustomAttribute<NonSerializedAttribute>())
            {
                return ePropertyConversionType.None;
            }

            if (propertyInfo.HasCustomAttribute<RequiredAttribute>())
            {
                return ePropertyConversionType.Required;
            }

            return ePropertyConversionType.Optional;
        }
    }
#endif

	public class AllPropertySelector : IPropertySelector
    {
        public ePropertyConversionType InclusionValue { get; }

        public AllPropertySelector(ePropertyConversionType inclusionValue = ePropertyConversionType.Optional)
        {
            this.InclusionValue = inclusionValue;
        }

        public ePropertyConversionType GetPropertyConversionType(PropertyInfo propertyInfo)
        {
            return this.InclusionValue;
        }
    }
}