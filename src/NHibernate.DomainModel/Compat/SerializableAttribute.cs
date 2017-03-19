#if !FEATURE_SERIALIZATION && NETSTANDARD
// ReSharper disable once CheckNamespace
namespace System
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    internal sealed class SerializableAttribute : Attribute
    {
    }
}
#endif
