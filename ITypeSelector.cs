using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AssemblyTypeLoader
{
    public interface ITypeSelector
    {
        bool ShouldKeepType(Type type);
    }

    public class DataContractTypeSelector : ITypeSelector
    {
        public bool ShouldKeepType(Type type)
        {
            return type.GetTypeInfo().HasCustomAttribute<DataContractAttribute>();
        }
    }

    public class NewtonsoftJsonTypeSelector : ITypeSelector
    {
        public bool ShouldKeepType(Type type)
        {
            return type.GetTypeInfo().HasCustomAttribute<JsonObjectAttribute>();
        }
    }

    public class SerializableTypeSelector : ITypeSelector
    {
        public bool ShouldKeepType(Type type)
        {
            return type.GetTypeInfo().HasCustomAttribute<SerializableAttribute>();
        }
    }

    public class AllTypesTypeSelector : ITypeSelector
    {
        public bool ShouldKeepType(Type type)
        {
            return true;
        }
    }
}